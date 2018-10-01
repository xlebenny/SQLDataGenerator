using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLDataGeneratorLibrary;
using System.Text.RegularExpressions;

namespace SQLDataGeneratorUnitTest
{
    [TestClass]
    public class TestDateTimeFormatter
    {
        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public void TestDateTimeFormatterCase1()
        {
            //
            //Arrange
            //
            var testRow = 100; //because random result, need more record to ensure it correct
            var format = @"insert into TableA \(\[ColumnA\]\) values"
                        + @"("
                        + @"\s*\('(2017-01-02|2017-01-09|2017-01-16|2017-01-23|2017-01-30) ([0][9]|[1][0-8]):([0-5][0-9]):([0-5][0-9])'\),*"
                        + @"){" + testRow + "}"
                        + @";"
                        + @"\s+GO"
                        ;
            var config = new GenerateConfig[]{
                new GenerateConfig {
                    TableName = "TableA",
                    ColumnName = "ColumnA",
                    DataType = "varchar",
                    GenerateFormat =  @"{dateTime|[\{ DayRange: '2017-01-01~2017-01-31',TimeRangeOfWeekDay:['mon:09:00:00~18:00:00']\} ]}",
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
                Regex.Match(result, format.ToString()).Success,
                "Format: \"{0}\", Actuals: \"{1}\"", format, result);
        }

        [TestMethod]
        public void TestDateTimeFormatterCase2()
        {
            //
            //Arrange
            //
            var testRow = 100; //because random result, need more record to ensure it correct
            var format = @"insert into TableA \(\[ColumnA\]\) values"
                        + @"("
                        + @"\s+\('"
                        + "("
                        + @"2017-01-02|2017-01-09|2017-01-16|2017-01-23|2017-01-30" //mon
                        + @"|2017-01-03|2017-01-10|2017-01-17|2017-01-24|2017-01-31" //tue
                        + @"|2017-01-04|2017-01-11|2017-01-18|2017-01-25" //wed
                        + @") ([0][9]|[1][0-8]):([0-5][0-9]):([0-5][0-9])'\),*"
                        + @"){" + testRow + "}"
                        + @";"
                        + @"\s+GO"
                        ;
            var config = new GenerateConfig[]{
                new GenerateConfig {
                    TableName = "TableA",
                    ColumnName = "ColumnA",
                    DataType = "varchar",
                    GenerateFormat =  @"{dateTime|[\{ DayRange: '2017-01-01~2017-01-31',TimeRangeOfWeekDay:['mon/tue/wed:09:00:00~18:00:00']\} ]}",
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
                Regex.Match(result, format.ToString()).Success,
                "Format: \"{0}\", Actuals: \"{1}\"", format, result);
        }

        [TestMethod]
        public void TestDateTimeFormatterCase3()
        {
            //
            //Arrange
            //
            var testRow = 100; //because random result, need more record to ensure it correct
            var format = @"insert into TableA \(\[ColumnA\]\) values"
                        + @"("
                        + @"\s+\('("
                        + @"((2017-01-02|2017-01-09|2017-01-16|2017-01-23|2017-01-30) ([0][9]|[1][0-8]):([0-5][0-9]):([0-5][0-9]))" //mon
                        + @"|((2017-01-07|2017-01-14|2017-01-21|2017-01-28) ([0][9]|[1][0-3]):([0-5][0-9]):([0-5][0-9]))" //sat
                        + @")'\),*"
                        + @"){" + testRow + "}"
                        + @";"
                        + @"\s+GO"
                        ;
            var config = new GenerateConfig[]{
                new GenerateConfig {
                    TableName = "TableA",
                    ColumnName = "ColumnA",
                    DataType = "varchar",
                    GenerateFormat =  @"{dateTime|[\{ DayRange: '2017-01-01~2017-01-31',TimeRangeOfWeekDay:['mon:09:00:00~18:00:00', 'sat:09:00:00~13:00:00']\} ]}",
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
                Regex.Match(result, format.ToString()).Success,
                "Format: \"{0}\", Actuals: \"{1}\"", format, result);
        }

        [TestMethod]
        public void TestDateTimeFormatterCase4()
        {
            //
            //Arrange
            //
            var testRow = 100; //because random result, need more record to ensure it correct
            var format = @"insert into TableA \(\[ColumnA\]\) values"
                        + @"("
                        + @"\s+\('("
                        + @"((2017-01-02|2017-01-09|2017-01-16|2017-01-23|2017-01-30) ([0][9]|[1][0-8]):([0-5][0-9]):([0-5][0-9]))" //mon
                        + @"|((2017-01-07|2017-01-14|2017-01-21|2017-01-28) ([0][9]|[1][0-3]):([0-5][0-9]):([0-5][0-9]))" //sat
                        + @"|((2017-03-07|2017-03-14|2017-03-21|2017-03-28) ([0][9]|[1][0-8]):([0-5][0-9]):([0-5][0-9]))" //tue
                        + @")'\),*"
                        + @"){" + testRow + "}"
                        + @";"
                        + @"\s+GO"
                        ;
            var config = new GenerateConfig[]{
                new GenerateConfig {
                    TableName = "TableA",
                    ColumnName = "ColumnA",
                    DataType = "varchar",
                    GenerateFormat =  @"{dateTime|[\{ DayRange: '2017-01-01~2017-01-31',TimeRangeOfWeekDay:['mon:09:00:00~18:00:00', 'sat:09:00:00~13:00:00']\},"
                                        + @"\{ DayRange: '2017-03-01~2017-03-31',TimeRangeOfWeekDay:['tue:09:00:00~18:00:00']\} ]}",
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
                Regex.Match(result, format.ToString()).Success,
                "Format: \"{0}\", Actuals: \"{1}\"", format, result);
        }
    }
}