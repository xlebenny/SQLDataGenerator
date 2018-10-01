using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLDataGeneratorApplication;
using System.Collections.Generic;
using System.Linq;

namespace SQLDataGeneratorUnitTest
{
    [TestClass]
    public class TestMainWindowViewModel
    {
        [TestInitialize]
        public void Initialize()
        {
            Configure.Init();
        }

        [TestMethod]
        public void TestColumnInformationUserInputParse()
        {
            //
            //Arrange
            //
            var vm = new MainWindowsViewModel();
            var userInputData =
@"
Staff	Id	bIgInt	nO	0
Staff	StaffCode	varchar	fAlsE	10
Staff	FirstName	nvarchar	yEs	50
Staff	LastName	nvarchar	TrUe	50
StaffLeave	Id	bIgInt	NO	0
StaffLeave	StaffId	bigint	NO	0
StaffLeave	LeaveTypeId	bigint	NO	0
StaffLeave	DateFrom	date	NO	0
StaffLeave	DateTo	date	NO	0
";

            //
            //Act
            //
            vm.ColumnInformationUserInput = userInputData;

            //
            //Assert
            //

            //count
            Assert.AreEqual(
                //last record no new line
                userInputData.Trim().Count(x => x == '\n') + 1, vm.GenerateConfigs.Count,
                "Parse record not match"
            );
            Assert.IsFalse(
                vm.GenerateConfigs.Any(x => x == null),
                "GenerateConfigs has null"
            );

            //record prase
            Assert.AreEqual(
                vm.GenerateConfigs[1].TableName, "Staff",
                false, "Prase Error (TableName)"
            );
            Assert.AreEqual(
               vm.GenerateConfigs[1].ColumnName, "StaffCode",
               false, "Prase Error (ColumnName)"
           );
            Assert.AreEqual(
               vm.GenerateConfigs[1].DataType, "varchar",
               false, "Prase Error (DataType)"
           );
            Assert.AreEqual(
               vm.GenerateConfigs[1].IsNullable, false,
               "Prase Error (IsNullable)"
           );
            Assert.AreEqual(
               vm.GenerateConfigs[1].CharacterMaximumLength, 10,
               "Prase Error (CharacterMaximumLength)"
           );

            //nullable
            Assert.AreEqual(
                vm.GenerateConfigs[0].IsNullable, false,
                "Nullable: nO --> false"
            );
            Assert.AreEqual(
                vm.GenerateConfigs[1].IsNullable, false,
                "Nullable: fAlSe --> false"
            );
            Assert.AreEqual(
                vm.GenerateConfigs[2].IsNullable, true,
                "Nullable: yEs --> true"
            );
            Assert.AreEqual(
                vm.GenerateConfigs[3].IsNullable, true,
                "Nullable: TrUe --> true"
            );

            //datatype lower case
            Assert.AreEqual(
                vm.GenerateConfigs[0].DataType, "bigint",
                false, "dataType haven't convert to lower case"
            );
        }

        [TestMethod]
        public void TestUpdateColumnInformation()
        {
            //
            //Arrange
            //
            var vm = new MainWindowsViewModel();
            var userInputData =
@"
Staff	Id	bIgInt	nO	0
Staff	StaffCode111	varchar	fAlsE	10
Staff	FirstName	nvarchar	yEs	50
";
            vm.ColumnInformationUserInput =
@"
Staff	Id	bIgInt	nO	0
Staff	StaffCode	varchar	fAlsE	10
Staff	FirstName	nvarchar	yEs	5
";

            //
            //Act
            //
            vm.ColumnInformationUserInput = userInputData;

            //
            //Assert
            //

            //count
            Assert.AreEqual(
               vm.GenerateConfigs[1].ColumnName, "StaffCode111",
                "Record update failture"
            );
        }

        [TestMethod]
        public void TestKeyInformationToGenerateRowCount()
        {
            //
            //Arrange
            //
            var vm = new MainWindowsViewModel();

            var userInputDatas = new List<TableConfig>
            {
                new TableConfig{ TableName = "Staff", GenerateRecordCount = 10},
                new TableConfig{ TableName = "LeaveType", GenerateRecordCount = 2},
                new TableConfig{ TableName = "StaffLeave", GenerateRecordCount = 100},
            };

            vm.ColumnInformationUserInput =
@"
Staff	Id	bigint	NO	0
Staff	StaffCode	varchar	NO	10
Staff	FirstName	nvarchar	YES	50
Staff	LastName	nvarchar	YES	50
LeaveType	Id	bigint	NO	0
LeaveType	LeaveCode	varchar	NO	10
LeaveType	Name	varchar	YES	50
StaffLeave	Id	bigint	NO	0
StaffLeave	StaffId	bigint	NO	0
StaffLeave	LeaveTypeId	bigint	NO	0
StaffLeave	DateFrom	date	NO	0
StaffLeave	DateTo	date	NO	0
";

            //
            //Act
            //
            userInputDatas.ForEach(x =>
                vm.TableConfigs.Single(y => y.TableName == x.TableName).GenerateRecordCount = x.GenerateRecordCount
            );

            //
            //Assert
            //
            userInputDatas.ForEach(x =>
                vm.GenerateConfigs.Where(y => y.TableName == x.TableName).ToList().ForEach(y =>
                    Assert.AreEqual(
                        x.GenerateRecordCount,
                        y.GenerateRecordCount
                    )
                )
            );
        }
    }
}