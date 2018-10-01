namespace SQLDataGeneratorApplication
{
    public class DatabaseConfig : ViewModelBase
    {
        private string _databaseName = string.Empty;

        public string DatabaseName
        {
            get => _databaseName; set => SetProperty(ref _databaseName, value);
        }
    }
}