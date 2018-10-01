using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SQLDataGeneratorLibrary
{
    //facade pattern
    public class SQLGenerator
    {
        public static void GenerateInsertStatement(ISQLBuilder builder, IEnumerable<GenerateConfig> configs, TextWriter writer)
        {
            var tables = FormatParser.Parse(configs);

            tables.GenerateInsertStatement(builder, writer);
        }

        public static StringBuilder GenerateInsertStatement(ISQLBuilder builder, IEnumerable<GenerateConfig> configs)
        {
            var result = new StringBuilder();

            using (var writer = new StringWriter(result))
            {
                GenerateInsertStatement(builder, configs, writer);
            }
            GC.Collect();

            return result;
        }
    }
}