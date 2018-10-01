using Benny.CSharpHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLDataGeneratorLibrary
{
    internal class FormatParser
    {
        public static FormatterTable Parse(IEnumerable<GenerateConfig> configs)
        {
            var tables = CreateFormatterTable(configs);

            return tables;
        }

        //Step 1: Parse {} to class
        //  Dear { dict|name=firstNameDict } { dict|name=lastNameDict }
        //Step 2: {} to string.format
        //  Dear {0} {1}
        private static KeyValuePair<string, FormatterNode> _parse(string format)
        {
            //inside column
            var tree = new FormatterNode { FormatString = format };

            for (int i = 0; i < format.Length; i++)
            {
                switch (format[i])
                {
                    case '\\':
                        i++; //skip next char
                        continue;

                    case '{':
                        var temp = _parse(format.Substring(i + 1));

                        //abc { dict|name=xxx } def --> abc {x} def
                        tree.FormatString = string.Format(
                                                    "{0}{1}{2}",
                                                    format.Substring(0, i),
                                                    FormatterNode.VariableString,
                                                    format.Substring(i + temp.Key.Length + 2) // 2 because { }
                                                );
                        tree.Children.Add(temp.Value);

                        format = tree.FormatString;
                        i = i + FormatterNode.VariableString.Length; //skip for text already prased

                        break;

                    case '}':
                        // dict|name=firstNameDict  not include { }
                        var formarString = format.Substring(0, i);

                        return
                            new KeyValuePair<string, FormatterNode>(
                                formarString,
                                new FormatterNode
                                {
                                    Children = new List<IFormatter>()
                                    {
                                        CreateFormatter(formarString)
                                    }
                                }
                            );
                    default: break;
                }
            }

            return new KeyValuePair<string, FormatterNode>(format, tree);
        }

        /// <summary>
        /// </summary>
        /// <param name="format">formatterName|args</param>
        /// <returns></returns>
        private static IFormatter CreateFormatter(string format)
        {
            var splitted = format.Split('|');
            var formatterName = splitted[0].Trim();
            var args = splitted[1].Trim()
                        //replace escapted from parser
                        .Replace(@"\{", "{").Replace(@"\}", "}")
                        //replace escapted from json
                        .Replace(@"""", @"\""");

            Type type;
            switch (formatterName)
            {
                //TODO factory pattern
                case "seq": type = typeof(SequentialNumberFormatter); break;
                case "ref": type = typeof(ReferenceColumnFormatter); break;
                case "dict": type = typeof(DictionaryFormatter); break;
                case "dateTime": type = typeof(DateTimeFormatter); break;
                default: throw new NotSupportedException($"formatterName: { formatterName }");
            }

            //FIXME: bad idea create self at instance, but i need set this to default behavior & no time to fix
            return ((IFormatter)Activator.CreateInstance(type)).CreateInstance(args);
        }

        private static FormatterTable CreateFormatterTable(IEnumerable<GenerateConfig> configs)
        {
            var tables = new List<Table>();

            foreach (var configByTable in configs.GroupBy(x => x.TableName))
            {
                var columns = new List<Column>();

                foreach (var column in configByTable)
                    columns.Add(
                        new Column
                        {
                            Formatter = _parse(column.GenerateFormat).Value,
                            ColumnName = column.ColumnName,
                            ColumnType = column.DataType,
                        }
                    );

                //TODO automapper?
                tables.Add(new Table { Columns = columns, TableName = configByTable.Key, GenerateRecordCount = configByTable.First().GenerateRecordCount, IdentityInsert = configByTable.First().IdentityInsert });
            }

            //sory by Dependency
            tables.ForEach(x =>
                x.DependencyOn = x.Columns
                                //find reference formatter
                                .Select(y => y.Formatter).OfType<FormatterNode>()
                                .SelectMany(y => y.GetAllChildren()).OfType<ReferenceColumnFormatter>()
                                .ToList()
            );
            tables.SelectMany(x => x.DependencyOn).ToList()
                .ForEach(x =>
                    tables.Single(y => y.TableName == x.Table).DependencyBy.Add(x) //two way
                );
            tables = tables.TSort(
                x => tables.Where(y => x.DependencyOn.Any(z => z.Table == y.TableName))
            ).ToList();

            //TODO it shouldn't created in here
            return new FormatterTable(configs.GetDatabaseConfig(), tables);
        }
    }
}