using System.Windows;
using System.Windows.Input;

namespace SQLDataGeneratorApplication
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class MessageDialog : Window
    {
        //ref https://stackoverflow.com/questions/2796470/wpf-create-a-dialog-prompt
        public MessageDialog(string message)
        {
            InitializeComponent();

            ResponseText = message;

            this.InputBindings.AddRange(new InputBinding[] {
                new InputBinding(new RelayCommand(x => OKButton_Click(null, null), x => true), new KeyGesture(Key.Enter))
            });
        }

        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }
    }
}