using System;
using System.Text;
using static SQLDataGeneratorLibrary.FormatterTable;

namespace SQLDataGeneratorLibrary
{
    public interface ISQLBuilder
    {
        string DatabaseName { get; }
        string ColumnInformationSQL { get; }
        Encoding Encoding { get; }

        StringBuilder BuildInsertStatement(FormatterTableHelper helper, Database database, Table table);
    }
}