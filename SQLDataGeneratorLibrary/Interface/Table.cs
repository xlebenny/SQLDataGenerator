using Benny.CSharpHelper;
using System.Collections.Generic;
using System.Linq;

namespace SQLDataGeneratorLibrary
{
    public class Table
    {
        public string TableName { get; set; }
        public List<Column> Columns { get; set; } = new List<Column>();
        public bool IdentityInsert { get; set; }
        public long GenerateRecordCount { get; set; }

        public List<ReferenceColumnFormatter> DependencyOn { get; set; } = new List<ReferenceColumnFormatter>(); //A need B, in B
        public List<ReferenceColumnFormatter> DependencyBy { get; set; } = new List<ReferenceColumnFormatter>(); //A need B, in A
        private Dictionary<string, int> ColumnIndexCache = new Dictionary<string, int>();

        public int FindColumnIndex(string name)
        {
            return ColumnIndexCache.GetValueOrAddDefault(
                    name,
                    Columns.IndexOf(Columns.Single(x => x.ColumnName == name))
                );
        }

        public override string ToString()
        {
            return $"TableName: { TableName }, Columns: [{ string.Join(", ", Columns) }], "
                + $"DependencyBy: [{ string.Join(", ", DependencyBy.Select(x => x.Table + "." + x.Column)) }], "
                + $"DependencyOn: [{ string.Join(", ", DependencyOn.Select(x => x.Table + "." + x.Column)) }]";
        }
    }
}