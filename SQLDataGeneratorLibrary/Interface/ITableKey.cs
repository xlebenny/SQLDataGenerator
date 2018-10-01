namespace SQLDataGeneratorLibrary
{
    public interface ITableKey
    {
        string TableName { get; set; }
    }

    public static class ITableKeyExtension
    {
        public static string GetKey(this ITableKey x)
        {
            return $"{ x.TableName }";
        }
    }
}