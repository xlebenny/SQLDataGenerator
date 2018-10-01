using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLDataGeneratorLibrary;
using System.Text.RegularExpressions;

namespace SQLDataGeneratorUnitTest
{
    [TestClass]
    public class TestSequentialNumberFormatter
    {
        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public void TestSequentialNumberFormatterCase1()
        {
            //
            //Arrange
            //
            var testRow = 4;
            var format = @"insert into TableA \(\[ColumnA\]\) values"
                            + @"\s+\('abc 010 def'\),"
                            + @"\s+\('abc 011 def'\),"
                            + @"\s+\('abc 012 def'\),"
                            + @"\s+\('abc 010 def'\)"
                            + @";"
                            + @"\s+GO"
                            ;
            var config = new GenerateConfig[]{
                new GenerateConfig {
                    TableName = "TableA",
                    ColumnName = "ColumnA",
                    DataType = "varchar",
                    GenerateFormat =  @"abc {seq|start:10,end:12,padding:'\{0:000\}'} def",
                    GenerateRecordCount = testRow
                }
            };

            //
            //Act
            //
            var result = SQLGenerator.GenerateInsertStatement(new MSSQLBuilder(), config).ToString();

            //
            //Assert
            //
            Assert.IsTrue(
                Regex.Match(result, format).Success,
                "Format: \"{0}\", Actuals: \"{1}\"", format, result);
        }
    }
}