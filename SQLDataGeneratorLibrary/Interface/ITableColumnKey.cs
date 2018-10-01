namespace SQLDataGeneratorLibrary
{
    public interface ITableColumnKey
    {
        string TableName { get; set; }
        string ColumnName { get; set; }
    }

    public static class ITableColumnKeyExtension
    {
        public static string GetKey(this ITableColumnKey x)
        {
            return $"{ x.TableName }-=-{ x.ColumnName }";
        }
    }
}