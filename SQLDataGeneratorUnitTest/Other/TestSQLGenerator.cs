using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLDataGeneratorLibrary;
using System.Text.RegularExpressions;

namespace SQLDataGeneratorUnitTest
{
    [TestClass]
    public class TestSQLGenerator
    {
        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public void TestSQLGeneratorCase1()
        {
            //
            //Arrange
            //
            var format =
                        @"use \[MyDB\];"
                        + @"\s+GO"
                        + @"\s+insert into TableA \(\[ColumnA\]\) values"
                            + @"\s+\('abc'\),"
                            + @"\s+\('abc'\)"
                        + @";"
                        + @"\s+GO"
                        + @"\s+use \[MyDB\];"
                        + @"\s+GO"
                        + @"\s+set IDENTITY_INSERT TableB on;"
                        + @"\s+GO"
                        + @"\s+insert into TableB \(\[ColumnB\]\) values"
                            + @"\s+\('def'\),"
                            + @"\s+\('def'\),"
                            + @"\s+\('def'\)"
                        + @";"
                        + @"\s+GO"
                        + @"\s+set IDENTITY_INSERT TableB off;"
                        + @"\s+GO"
                        ;
            var config = new GenerateConfig[]{
                new GenerateConfig {
                    DatabaseName = "MyDB",
                    TableName = "TableA",
                    ColumnName = "ColumnA",
                    DataType = "varchar",
                    GenerateFormat =  @"abc",
                    GenerateRecordCount = 2
                },
                new GenerateConfig {
                    DatabaseName = "MyDB",
                    TableName = "TableB",
                    ColumnName = "ColumnB",
                    DataType = "varchar",
                    GenerateFormat =  @"def",
                    GenerateRecordCount = 3,
                    IdentityInsert = true
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
                Regex.Match(result, format.ToString()).Success,
                "Format: \"{0}\", Actuals: \"{1}\"", format, result);
        }
    }
}