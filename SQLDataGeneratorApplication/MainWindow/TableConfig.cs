using SQLDataGeneratorLibrary;

namespace SQLDataGeneratorApplication
{
    public class TableConfig : ViewModelBase, ITableKey
    {
        public string TableName { get; set; }
        public bool _identityInsert = false;
        private long _generateRecordCount = 0;

        public long GenerateRecordCount
        {
            get => _generateRecordCount; set => SetProperty(ref _generateRecordCount, value);
        }

        public bool IdentityInsert
        {
            get => _identityInsert; set => SetProperty(ref _identityInsert, value);
        }
    }
}