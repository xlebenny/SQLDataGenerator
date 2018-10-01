using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLDataGeneratorLibrary;
using System.Text.RegularExpressions;

namespace SQLDataGeneratorUnitTest
{
    [TestClass]
    public class TestDictionaryFormatter
    {
        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public void TestDictionaryFormatterCase1()
        {
            //
            //Arrange
            //
            var testRow = 1;
            var format = @"insert into TableA \(\[ColumnA\]\) values\s+\('abc (M|F) def'\)"
                        + @";"
                        + @"\s+GO"
                        ;
            var config = new GenerateConfig[]{
                new GenerateConfig {
                    TableName = "TableA",
                    ColumnName = "ColumnA",
                    DataType = "varchar",
                    GenerateFormat =  "abc { dict|name:'firstNameDict',field:'Gender' } def",
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

        [TestMethod]
        public void TestDictionaryFormatterCase2()
        {
            //
            //Arrange
            //
            var testRow = 1;
            var format = @"insert into TableA \(\[ColumnA\]\) values\s+\('Dear \w+ \w+'\)"
                        + @";"
                        + @"\s+GO"
                        ;
            var config = new GenerateConfig[]{
                new GenerateConfig {
                    TableName = "TableA",
                    ColumnName = "ColumnA",
                    DataType = "varchar",
                    GenerateFormat =  "Dear { dict|name:'firstNameDict',field:'Eng' } { dict|name:'LastNameDict',field:'Eng' }",
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

        [TestMethod]
        public void TestDictionaryFormatterCase3()
        {
            //
            //Arrange
            //
            var testRow = 1;
            var format = @"insert into TableA \(\[ColumnA\]\) values\s+\('Dear {abc} \w+ \w+'\)"
                        + @";"
                        + @"\s+GO"
                        ;
            var config = new GenerateConfig[]{
                new GenerateConfig {
                    TableName = "TableA",
                    ColumnName = "ColumnA",
                    DataType = "varchar",
                    GenerateFormat =  "Dear \\{abc\\} { dict|name:'firstNameDict',field:'Eng' } { dict|name:'LastNameDict',field:'Eng' }",
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

        [TestMethod]
        public void TestDictionaryFormatterCase4()
        {
            //
            //Arrange
            //
            var testRow = 1;
            var format = @"insert into TableA \(\[ColumnA\]\) values\s+\('abc def'\)"
                        + @";"
                        + @"\s+GO"
                        ;
            var config = new GenerateConfig[]{
                new GenerateConfig {
                    TableName = "TableA",
                    ColumnName = "ColumnA",
                    DataType = "varchar",
                    GenerateFormat =  "abc def",
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

        [TestMethod]
        public void TestDictionaryFormatterCase5()
        {
            //
            //Arrange
            //
            var testRow = 1;
            var format = @"insert into TableA \(\[ColumnA\], \[Gender\]\) values\s+\('\w+ \w+', '(M|F)'\)"
                        + @";"
                        + @"\s+GO"
                        ;
            var config = new GenerateConfig[]{
                new GenerateConfig {
                    TableName = "TableA",
                    ColumnName = "ColumnA",
                    DataType = "varchar",
                    GenerateFormat =  "{  dict|name:'firstNameDict',field:'Eng' } { dict|name:'LastNameDict',field:'Eng' }",
                    GenerateRecordCount = testRow
                },
                 new GenerateConfig {
                    TableName = "TableA",
                    ColumnName = "Gender",
                    DataType = "varchar",
                    GenerateFormat =  "{  dict|name:'firstNameDict',field:'Gender' }",
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

        [TestMethod]
        public void TestDictionaryFormatterCase6()
        {
            //
            //Arrange
            //
            var testRow = 2;
            var format = @"insert into TableA \(\[ColumnA\]\) values"
                        + @"\s+\('(\w+)'\),"
                        + @"\s+\('(\w+)'\)"
                        + @";"
                        + @"\s+GO"
                        ;
            var config = new GenerateConfig[]{
                new GenerateConfig {
                    TableName = "TableA",
                    ColumnName = "ColumnA",
                    DataType = "varchar",
                    GenerateFormat =  "{  dict|name:'firstNameDict',field:'Eng' }",
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
            Assert.AreNotEqual(
                Regex.Match(result, format).Groups[1].Value, Regex.Match(result, format).Groups[2].Value,
                "Format: \"{0}\", Actuals: \"{1}\"", format, result);
        }
    }
}