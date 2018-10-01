using SQLDataGeneratorLibrary;

namespace SQLDataGeneratorLibrary
{
    public class Column
    {
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public IFormatter Formatter { get; set; }
    }
}