using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SQLDataGeneratorApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };

        public MainWindow()
        {
            InitializeComponent();

            //ref https://stackoverflow.com/questions/48935/how-can-i-register-a-global-hot-key-to-say-ctrlshiftletter-using-wpf-and-ne
            this.InputBindings.AddRange(new InputBinding[] {
                new InputBinding(new RelayCommand(x => Load(), x => true), new KeyGesture(Key.O, ModifierKeys.Control)),
                new InputBinding(new RelayCommand(x => Save(), x => true), new KeyGesture(Key.S, ModifierKeys.Control)),
            });
        }

        private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void OnLoadButtonClicked(object sender, RoutedEventArgs e)
        {
            Load();
        }

        //
        // Save / load Config
        // because need to replace VM, so this method can't inside VM
        //
        private FileDialog GetFileDialog(bool saveDialog)
        {
            var dialog = saveDialog ? new SaveFileDialog() : new OpenFileDialog() as FileDialog;

            dialog.FileName = "SQLGenerator.cfg";
            dialog.Filter = "Configure File | *.cfg";

            return dialog;
        }

        private void Save()
        {
            var dialog = GetFileDialog(true);
            if (dialog.ShowDialog() == true)
                File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(this.DataContext, JsonSerializerSettings));
        }

        private void Load()
        {
            var dialog = GetFileDialog(false);
            if (dialog.ShowDialog() == true)
            {
                var cfg = File.ReadAllText(dialog.FileName);
                this.DataContext = JsonConvert.DeserializeObject(cfg, this.DataContext.GetType(), JsonSerializerSettings);
            }
        }
    }
}