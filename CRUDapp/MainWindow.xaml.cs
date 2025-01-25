using CRUDapp.Pages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace CRUDapp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack) MainFrame.GoBack();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите выйти?",
                                                      "Подтверждение выхода",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                e.Cancel = true;
            base.OnClosing(e);
        }

        private void MainFrame_OnNavigated(object sender, NavigationEventArgs e)
        {
            if (!(e.Content is Page page)) return;
            this.Title = $"ProjectByMartoyan {page.Title}";

            if (page is AuthPage) ButtonBack.Visibility = Visibility.Hidden;
            else ButtonBack.Visibility = Visibility.Visible;
        }
    }
}
