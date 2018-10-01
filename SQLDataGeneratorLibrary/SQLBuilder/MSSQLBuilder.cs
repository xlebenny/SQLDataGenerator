using System.Linq;
using System.Text;
using static SQLDataGeneratorLibrary.FormatterTable;

namespace SQLDataGeneratorLibrary
{
    internal class MSSQLBuilder : ISQLBuilder
    {
        private static readonly string[] QuoteDataType = new string[]
        {
            "varchar", "nvarchar", "char", "nvchar", "text", "xml", "date", "time", "dateTime", "dateTime2"
        };

        private const long BatchSize = 1000;
        private const string StringQuoteFormat = "'{0}'";
        private const string EscapeColumnNameFormat = "[{0}]";

        public string DatabaseName => "Microsoft SQL Server";

        public string ColumnInformationSQL =>
@"
select [TABLE_NAME], [COLUMN_NAME], [DATA_TYPE], [IS_NULLABLE], isnull([CHARACTER_MAXIMUM_LENGTH], 0) from INFORMATION_SCHEMA.COLUMNS where [TABLE_NAME] != 'sysdiagrams'
";

        public Encoding Encoding => Encoding.Unicode;

        public StringBuilder BuildInsertStatement(FormatterTableHelper helper, Database database, Table table)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(database.Name))
            {
                sb.AppendLine($"use [{ database.Name }];");
                sb.AppendLine("GO");
                sb.AppendLine();
            }
            if (table.IdentityInsert)
            {
                sb.AppendLine($"set IDENTITY_INSERT { table.TableName } on;");
                sb.AppendLine("GO");
                sb.AppendLine();
            }

            for (var i = 0L; i < helper.CurrentDataTable.LongLength; i++)
            {
                var startBatch = i % BatchSize == 0;
                var endBatch = (i + 1) % BatchSize == 0 || (i + 1) == helper.CurrentDataTable.LongLength;

                if (startBatch)
                {
                    var insertStatement = string.Format(
                                                "insert into {0} ({1}) values",
                                                table.TableName,
                                                string.Join(", ", table.Columns.Select(x => string.Format(EscapeColumnNameFormat, x.ColumnName)))
                    );
                    sb.AppendLine(insertStatement);
                }

                //    (a, b, c, d, e)
                var values = new string[table.Columns.Count];
                for (var j = 0; j < table.Columns.Count; j++)
                {
                    var column = table.Columns[j];
                    var value = helper.CurrentDataTable[i][j];

                    values[j] = this.QuoteIfNeeded(column, value);
                }
                sb.Append(string.Format("   ({0})", string.Join(", ", values)));

                if (endBatch)
                {
                    sb.AppendLine(";");
                    sb.AppendLine("GO");
                    sb.AppendLine();
                }
                else
                    sb.Append(",");

                sb.AppendLine();
            }

            if (table.IdentityInsert)
            {
                sb.AppendLine($"set IDENTITY_INSERT { table.TableName } off;");
                sb.AppendLine("GO");
                sb.AppendLine();
            }

            return sb;
        }

        private string QuoteIfNeeded(Column column, string value)
        {
            return QuoteDataType.Contains(column.ColumnType)
                    ? string.Format(StringQuoteFormat, value)
                    : value;
        }
    }
}