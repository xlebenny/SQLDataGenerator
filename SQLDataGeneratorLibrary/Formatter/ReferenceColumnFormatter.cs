using Benny.CSharpHelper;
using System;
using System.Linq;
using System.Reflection;
using static SQLDataGeneratorLibrary.FormatterTable;

namespace SQLDataGeneratorLibrary
{
    public class ReferenceColumnFormatter : IFormatter
    {
        public string Table { get; set; }
        public string Column { get; set; }
        public string ConverterName { get; set; }
        public object[] ConverterParams { get; set; }

        private static readonly Random random = new Random();
        private static readonly string Namespace = Assembly.GetExecutingAssembly().GetName().Name;
        private const string Key = "(ReferenceFormatterKey-=-{0}.{1})";

        private long ThisColumnIndex; //for remember this column belong from
        private IReferenceFormatterConverter Conveter;

        public override void BeforeTableExecute(FormatterTableHelper helper)
        {
            if (!string.IsNullOrEmpty(ConverterName))
            {
                Conveter = (IReferenceFormatterConverter)Activator.CreateInstance(Type.GetType($"{ Namespace }.{ ConverterName }"));
                if (Conveter == null) throw new NotSupportedException($"[ReferenceColumnFormatter] { ConverterName } not found");
            }
        }

        //two case
        //1. if it's another table, get random row
        public override string FormatField(FormatterTableHelper helper)
        {
            if (IsReferenceCurrentTable(helper.CurrentTable))
            {
                ThisColumnIndex = helper.CurrentColumnIndex;
                return ReferenceFormatterKey(Table, Column); //do it after row execute
            }
            else
            {
                var column = (string[])helper.Session[ReferenceFormatterKey(Table, Column)];
                var value = column[random.Next(0, column.LongLength)];
                var convertedValue = ConvertValueIfNeeded(value);

                return convertedValue;
            }
        }

        //2. if it's itself, get current row
        public override void AfterRowExecute(FormatterTableHelper helper)
        {
            if (IsReferenceCurrentTable(helper.CurrentTable))
            {
                //replace value before generate statement
                var columIndex = helper.CurrentTable.FindColumnIndex(Column);
                var value = helper.CurrentDataTable[helper.CurrentRowIndex][ThisColumnIndex].Replace(
                                ReferenceFormatterKey(Table, Column),
                                helper.CurrentDataTable[helper.CurrentRowIndex][columIndex]
                            );
                var convertedValue = ConvertValueIfNeeded(value);

                helper.CurrentDataTable[helper.CurrentRowIndex][ThisColumnIndex] = convertedValue;
            }
        }

        //save session if needed
        public override void DependencyByTableExecuted(FormatterTableHelper helper)
        {
            var table = helper.CurrentTable;
            var dependencyBy = table.DependencyBy.Where(x => !table.DependencyOn.Contains(x)).ToArray(); //if by & be contain that, it's reference itself

            foreach (var referenceColumnFormatter in dependencyBy)
            {
                var key = ReferenceFormatterKey(referenceColumnFormatter.Table, referenceColumnFormatter.Column);
                var columnValues = new string[helper.CurrentDataTable.LongLength];
                var columIndex = helper.CurrentTable.FindColumnIndex(referenceColumnFormatter.Column);

                //just clone that column
                for (var rowIndex = 0L; rowIndex < columnValues.LongLength; rowIndex++)
                    columnValues[rowIndex] = helper.CurrentDataTable[rowIndex][columIndex];

                helper.Session.Add(key, columnValues);
            }
        }

        private string ConvertValueIfNeeded(string value)
        {
            var result = null as string;

            result = (Conveter != null)
                        ? Conveter.Convert(value, ConverterParams)
                        : value;

            return result;
        }

        private bool IsReferenceCurrentTable(Table table)
        {
            return IsReferenceCurrentTable(table.TableName);
        }

        private bool IsReferenceCurrentTable(string tableName)
        {
            return tableName == Table;
        }

        public static string ReferenceFormatterKey(string tableName, string columnName)
        {
            return string.Format(Key, tableName, columnName);
        }

        public override string ToString()
        {
            return $"{ Table }.{ Column }";
        }
    }
}