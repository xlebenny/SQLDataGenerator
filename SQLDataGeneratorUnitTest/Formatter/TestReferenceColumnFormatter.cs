using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLDataGeneratorLibrary;
using System.Text.RegularExpressions;

namespace SQLDataGeneratorUnitTest
{
    [TestClass]
    public class TestReferenceColumnFormatter
    {
        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public void TestReferenceColumnFormatterCase1()
        {
            //
            //Arrange
            //
            var testRow = 2;
            var format = @"insert into Staff \(\[Id\]\) values"
                        + @"\s*\('E001'\),*"
                        + @"\s*\('E002'\),*"
                        + @";"
                        + @"\s+GO"
                        + @"\s*insert into LeaveType \(\[Id\]\) values"
                        + @"\s*\('L001'\),*"
                        + @"\s*\('L002'\),*"
                        + @";"
                        + @"\s+GO"
                        + @"\s*insert into StaffLeave \(\[Id\], \[StaffId\], \[LeaveId\]\) values"
                        + @"\s*\('1', 'E00(1|2)', 'L00(1|2)'\),*"
                        + @"\s*\('2', 'E00(1|2)', 'L00(1|2)'\),*"
                        + @";"
                        + @"\s+GO"
                        ;
            var config = new GenerateConfig[]{
                new GenerateConfig {
                    TableName = "Staff",
                    ColumnName = "Id",
                    DataType = "varchar",
                    GenerateFormat =  @"E{seq|start:1,padding:'\{0:000\}'}",
                    GenerateRecordCount = testRow
                },
                new GenerateConfig {
                    TableName = "StaffLeave",
                    ColumnName = "Id",
                    DataType = "varchar",
                    GenerateFormat =  @"{seq|start:1}",
                    GenerateRecordCount = testRow
                },
                new GenerateConfig {
                    TableName = "StaffLeave",
                    ColumnName = "StaffId",
                    DataType = "varchar",
                    GenerateFormat =  @"{ref|table:'Staff',column:'Id'}",
                    GenerateRecordCount = testRow
                },
                new GenerateConfig {
                    TableName = "StaffLeave",
                    ColumnName = "LeaveId",
                    DataType = "varchar",
                    GenerateFormat =  @"{ref|table:'LeaveType',column:'Id'}",
                    GenerateRecordCount = testRow
                },
                new GenerateConfig {
                    TableName = "LeaveType",
                    ColumnName = "Id",
                    DataType = "varchar",
                    GenerateFormat =  @"L{seq|start:1,padding:'\{0:000\}'}",
                    GenerateRecordCount = testRow
                },
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
        public void TestReferenceColumnFormatterCase2()
        {
            //
            //Arrange
            //
            var testRow = 2;
            var format = @"insert into Staff \(\[Id\], \[FirstName\], \[LastName\], \[FullName\]\) values"
                        + @"\s*\('E001', 'Benny001', 'Leung001', 'Benny001 Leung001'\),*"
                        + @"\s*\('E002', 'Benny002', 'Leung002', 'Benny002 Leung002'\),*"
                        + @";"
                        + @"\s+GO"
                        ;
            var config = new GenerateConfig[]{
                new GenerateConfig {
                    TableName = "Staff",
                    ColumnName = "Id",
                    DataType = "varchar",
                    GenerateFormat =  @"E{seq|start:1,padding:'\{0:000\}'}",
                    GenerateRecordCount = testRow
                },
                new GenerateConfig {
                    TableName = "Staff",
                    ColumnName = "FirstName",
                    DataType = "varchar",
                    GenerateFormat =  @"Benny{seq|start:1,padding:'\{0:000\}'}",
                    GenerateRecordCount = testRow
                },
                new GenerateConfig {
                    TableName = "Staff",
                    ColumnName = "LastName",
                    DataType = "varchar",
                    GenerateFormat =  @"Leung{seq|start:1,padding:'\{0:000\}'}",
                    GenerateRecordCount = testRow
                },
                new GenerateConfig {
                    TableName = "Staff",
                    ColumnName = "FullName",
                    DataType = "varchar",
                    GenerateFormat =  @"{ref|table:'Staff',column:'FirstName'} {ref|table:'Staff',column:'LastName'}",
                    GenerateRecordCount = testRow
                },
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
        public void TestReferenceColumnFormatterCase3()
        {
            //
            //Arrange
            //
            var testRow = 1;
            var format = @"insert into StaffLeave \(\[Id\], \[StartDate\], \[EndDate\], \[DayTaken\]\) values"
                        + @"\s*\('L001', '2017-01-01 00:00:00', '2017-01-02 00:00:00', '1'\),*"
                        + @";"
                        + @"\s+GO"
                        ;
            var config = new GenerateConfig[]{
                new GenerateConfig {
                    TableName = "StaffLeave",
                    ColumnName = "Id",
                    DataType = "varchar",
                    GenerateFormat =  @"L{seq|start:1,padding:'\{0:000\}'}",
                    GenerateRecordCount = testRow
                },
                new GenerateConfig {
                    TableName = "StaffLeave",
                    ColumnName = "StartDate",
                    DataType = "varchar",
                    GenerateFormat =  @"2017-01-01 00:00:00",
                    GenerateRecordCount = testRow
                },
                new GenerateConfig {
                    TableName = "StaffLeave",
                    ColumnName = "EndDate",
                    DataType = "varchar",
                    GenerateFormat =  @"{ref|table:'StaffLeave',column:'StartDate',converterName:'DateTimeConverter',converterParams:['AddDays', 1]}",
                    GenerateRecordCount = testRow
                },
                new GenerateConfig {
                    TableName = "StaffLeave",
                    ColumnName = "DayTaken",
                    DataType = "varchar",
                    GenerateFormat =  @"1",
                    GenerateRecordCount = testRow
                },
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