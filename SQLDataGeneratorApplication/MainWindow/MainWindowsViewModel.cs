using AutoMapper;
using Benny.CSharpHelper;
using Microsoft.Win32;
using SQLDataGeneratorLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SQLDataGeneratorApplication
{
    public class MainWindowsViewModel : ViewModelBase
    {
        public MainWindowsViewModel()
        {
            SqlBuilders = new ObservableCollection<ISQLBuilder>(TypeHelper.GetImplementsOf<ISQLBuilder>());
            ColumnInformations = new ObservableCollection<ColumnInformation>();
            GenerateConfigs = new ObservableCollection<GenerateConfig>();
            TableConfigs = new ObservableCollection<TableConfig>();
            DatabaseConfig = new DatabaseConfig();
        }

        //
        //Step 1.
        //

        private ObservableCollection<ISQLBuilder> _sqlBuilders;
        private ISQLBuilder _selectedSQLBuilder;
        private string _columnInformationUserInput = string.Empty;
        private ObservableCollection<ColumnInformation> _columnInformations;

        public ICommand ShowColumnInformationSQL => new RelayCommand(x => new MessageDialog(SelectedSQLBuilder.ColumnInformationSQL).Show(), x => true);

        //
        //Step 2.
        //

        private ObservableCollection<GenerateConfig> _generateConfigs;

        //
        //Step 3.
        //
        private DatabaseConfig _databaseConfig;

        private ObservableCollection<TableConfig> _tableConfigs;

        //
        //Step 4.
        //

        private bool _generating = false;

        public ICommand Generate => new RelayCommand(
                    x =>
                    {
                        Generating = true;
                        Task.Run(() =>
                        {
                            var dialog = new SaveFileDialog()
                            {
                                FileName = "InsertStatement.sql",
                                Filter = "SQL File | *.sql"
                            };

                            if (dialog.ShowDialog() == true)
                            {
                                using (var streamWriter = new StreamWriter(dialog.FileName, false, SelectedSQLBuilder.Encoding))
                                {
                                    //file too lager, StringBuilder will OutOfMemoryException
                                    SQLGenerator.GenerateInsertStatement(SelectedSQLBuilder, this.GenerateConfigs, streamWriter);
                                }
                                GC.Collect(); //force release memory
                            }

                            Generating = false;
                        });
                    },
                    x => this.GenerateConfigs.Count > 0 && !Generating && SelectedSQLBuilder != null
                );

        //
        //OnPropertyChanged
        //

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            switch (propertyName)
            {
                case "ColumnInformationUserInput": RefreshColumnInformations(); break;
                case "ColumnInformations": RefreshGenerateConfigs(); break;
                case "DatabaseConfig": RefreshGenerateConfigs(); break;
                case "GenerateConfigs": RefreshTableConfigs(); break;
                case "TableConfigs": Mapper.Map(this.TableConfigs, this.GenerateConfigs); break;
                default: break;
            }

            base.OnPropertyChanged(propertyName);
        }

        private void RefreshColumnInformations()
        {
            var result = CSVHelper.ToObject(
                new Expression<Func<ColumnInformation, dynamic>>[] { x => x.TableName, x => x.ColumnName, x => x.DataType, x => x.IsNullable, x => x.CharacterMaximumLength },
                this.ColumnInformationUserInput,
                '\t'
            );

            this.ColumnInformations = new ObservableCollection<ColumnInformation>(result);
        }

        private void RefreshGenerateConfigs()
        {
            var result = Mapper.Map<List<GenerateConfig>>(this.ColumnInformations);

            Mapper.Map(this.GenerateConfigs, result);
            Mapper.Map(this.DatabaseConfig, result);
            Mapper.Map(this.TableConfigs, result);

            this.GenerateConfigs = new ObservableCollection<GenerateConfig>(result);
        }

        private void RefreshTableConfigs()
        {
            var result = Mapper.Map<List<TableConfig>>(this.GenerateConfigs);
            this.TableConfigs = new ObservableCollection<TableConfig>(result);
        }

        #region Getter Setter

        public string ColumnInformationUserInput
        {
            get => _columnInformationUserInput; set => SetProperty(ref _columnInformationUserInput, value);
        }

        public ObservableCollection<ColumnInformation> ColumnInformations
        {
            get => _columnInformations; set => SetProperty(ref _columnInformations, value);
        }

        public ObservableCollection<GenerateConfig> GenerateConfigs
        {
            get => _generateConfigs; set => SetProperty(ref _generateConfigs, value);
        }

        public bool Generating
        {
            get => _generating; set => SetProperty(ref _generating, value);
        }

        public ObservableCollection<TableConfig> TableConfigs
        {
            get => _tableConfigs; set => SetProperty(ref _tableConfigs, value);
        }

        public ObservableCollection<ISQLBuilder> SqlBuilders
        {
            get => _sqlBuilders; set => SetProperty(ref _sqlBuilders, value);
        }

        public ISQLBuilder SelectedSQLBuilder
        {
            get => _selectedSQLBuilder; set => SetProperty(ref _selectedSQLBuilder, value);
        }

        public DatabaseConfig DatabaseConfig
        {
            get => _databaseConfig; set => SetProperty(ref _databaseConfig, value);
        }

        #endregion Getter Setter
    }
}