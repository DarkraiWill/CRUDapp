using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CRUDapp.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {

        public AuthPage()
        {
            InitializeComponent();
            this.Loaded += AuthPage_Loaded; // Обработчик загрузки страницы
        }

        private void AuthPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Очистка полей
            log.Clear();
            pass.Clear();
            txtHintLogin.Visibility = Visibility.Visible;
            txtHintPass.Visibility = Visibility.Visible;

            // Сделать кнопку регистрации видимой
            ButtonReg.Visibility = Visibility.Visible;
        }

        private void log_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtHintLogin.Visibility = Visibility.Visible;
            if (log.Text.Length > 0)
            {
                txtHintLogin.Visibility = Visibility.Hidden;
            }
        }

        private void txtHintLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            log.Focus();
        }

        private void pass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtHintPass.Visibility = Visibility.Visible;
            if (pass.Password.Length > 0)
            {
                txtHintPass.Visibility = Visibility.Hidden;
            }
        }

        private void txtHintPass_MouseDown(object sender, MouseButtonEventArgs e)
        {
            pass.Focus();
        }

        private void ButtonGoIn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(log.Text) || string.IsNullOrEmpty(pass.Password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка", MessageBoxButton.OK);
                return;
            }

            string hashedPassword = GetHash(pass.Password); // Хэшируем пароль перед сравнением

            using (var db = new ToDoListEntities())
            {
                var user = db.users.AsNoTracking()
                    .FirstOrDefault(u => u.username == log.Text && u.password_hash == hashedPassword);

                if (user == null)
                {
                    MessageBox.Show("Пользователь с такими данными не найден!");
                    return;
                }

                MessageBox.Show("Пользователь найден!");

                switch (user.role)
                {
                    case "admin":
                        NavigationService?.Navigate(new AdminMenu());
                        break;
                    case "user":
                        NavigationService?.Navigate(new UserMenu(user.user_id));
                        break;
                }
            }
        }

        private void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegPage(null));
        }

        // Метод для хеширования пароля
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create()) // Используем алгоритм SHA1 для хеширования
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)) // Хешируем пароль
                    .Select(x => x.ToString("X2"))); // Преобразуем байты в строку в шестнадцатеричном формате
            }
        }
    }
}
