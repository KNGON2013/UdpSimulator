using System.Windows;

namespace UdpSimulator.Xamls
{
    /// <summary>
    /// ErrorDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ErrorDialog : Window
    {
        public ErrorDialog()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.buttonOK.Focus();
        }
    }
}
