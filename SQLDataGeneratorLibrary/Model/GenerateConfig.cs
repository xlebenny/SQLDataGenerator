using System.Collections.Generic;
using System.Linq;

namespace SQLDataGeneratorLibrary
{
    public class GenerateConfig : ITableColumnKey, ITableKey
    {
        public string DatabaseName { get; set; }
        public bool IdentityInsert { get; set; }

        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string GenerateFormat { get; set; } = string.Empty;

        public string DataType { get; set; }
        public bool IsNullable { get; set; }
        public long CharacterMaximumLength { get; set; }

        public long GenerateRecordCount { get; set; }

        public override string ToString()
        {
            return $"{ TableName } { ColumnName }";
        }
    }

    public static class GenerateConfigExtension
    {
        public static Database GetDatabaseConfig(this IEnumerable<GenerateConfig> configs)
        {
            return new Database { Name = configs.First().DatabaseName };
        }
    }
}