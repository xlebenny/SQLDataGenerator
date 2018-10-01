using SQLDataGeneratorLibrary;

namespace SQLDataGeneratorApplication
{
    public class ColumnInformation : ITableColumnKey
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string IsNullable { get; set; }
        public long CharacterMaximumLength { get; set; }
    };
}