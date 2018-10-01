using Benny.CSharpHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SQLDataGeneratorLibrary
{
    public class FormatterTable
    {
        private FormatterTableHelper helper;

        public FormatterTable(Database database, List<Table> tables)
        {
            helper = new FormatterTableHelper(database, tables);
        }

        public void GenerateInsertStatement(ISQLBuilder builder, TextWriter writer)
        {
            helper.GenerateStatement(builder.BuildInsertStatement, writer);
        }

        public class FormatterTableHelper
        {
            public List<Table> Tables { get; }
            public Database Database { get; }
            public IDictionary<string, object> Session { get; } = new Dictionary<string, object>();

            //
            //prevent use to much RAM, below item clean after generated for each table
            //

            public Table CurrentTable { get; set; }
            public Column CurrentColumn { get; set; }
            public long CurrentRowIndex { get; set; }
            public int CurrentColumnIndex { get; set; }

            //ref https://stackoverflow.com/questions/468832/why-are-multi-dimensional-arrays-in-net-slower-than-normal-arrays
            public string[][] CurrentDataTable { get; set; } //row / column, for perforamce, no list/object/rectangleArray

            private FormatterTableHelper()
            {
            }

            public FormatterTableHelper(Database database, List<Table> tables)
            {
                Database = database;
                Tables = tables;
            }

            public string FormatField(Column column)
            {
                CurrentColumn = column;

                return FormatField(column.Formatter);
            }

            public string FormatField(IFormatter formatter)
            {
                string result = null;

                formatter.BeforeFieldExecute(this);
                result = formatter.FormatField(this);
                formatter.AfterFieldExecute(this);

                return result;
            }

            public void GenerateStatement(
                Func<FormatterTableHelper, Database, Table, StringBuilder> statementWrapper,
                TextWriter writer
            )
            {
                //var statements = new StringBuilder();

                foreach (var table in Tables)
                {
                    //init
                    this.CurrentDataTable = ArrayHelper.CreateJaggedArray<string[][]>(table.GenerateRecordCount, table.Columns.Count);
                    this.CurrentTable = table;

                    //generate
                    //dataTabe value may change at each moment, (not only FormatField)
                    //if debug please check each step
                    table.Columns.ForEach(x => x.Formatter.BeforeTableExecute(this));
                    for (CurrentRowIndex = 0L; CurrentRowIndex < CurrentDataTable.LongLength; CurrentRowIndex++)
                    {
                        table.Columns.ForEach(x => x.Formatter.BeforeRowExecute(this));
                        for (CurrentColumnIndex = 0; CurrentColumnIndex < table.Columns.Count; CurrentColumnIndex++)
                            CurrentDataTable[CurrentRowIndex][CurrentColumnIndex] = this.FormatField(table.Columns[CurrentColumnIndex]);
                        table.Columns.ForEach(x => x.Formatter.AfterRowExecute(this));
                    }
                    table.Columns.ForEach(x => x.Formatter.AfterTableExecute(this));

                    table.DependencyBy.ForEach(x => x.DependencyByTableExecuted(this));

                    //To statement per table, use less RAM
                    //statements.Append(statementWrapper(this, Database, table));
                    writer.Write(statementWrapper(this, Database, table));
                }

                //return statements;
            }
        }
    }
}