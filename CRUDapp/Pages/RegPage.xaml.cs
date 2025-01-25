using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace CRUDapp.Pages
{
    public partial class RegPage : Page
    {
        private user _currentUser = new user();
        private ToDoListEntities db = new ToDoListEntities(); // Подключение к базе данных через Entity Framework
        
        public RegPage(user selectedUser)
        {
            InitializeComponent();
            if (selectedUser != null)
            {
                _currentUser = selectedUser;
                // Заполняем поля формы данными выбранного пользователя
                tbLogin.Text = _currentUser.username;
                tbPassword.Password = _currentUser.password_hash;
                tbConfirmPassword.Password = _currentUser.password_hash;
                ComboRole.Text = _currentUser.role;
            }
            DataContext = _currentUser;
        }

        private void Button_Click(object sender, RoutedEventArgs e) 
        {
            NavigationService?.GoBack();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // Кнопка "Регистрация"
        {
            string login = ((TextBox)FindName("tbLogin"))?.Text.Trim();
            string password = ((PasswordBox)FindName("tbPassword"))?.Password; 
            string confirmPassword = ((PasswordBox)FindName("tbConfirmPassword"))?.Password; 
            string role = ((ComboBox)FindName("ComboRole"))?.Text; 
            
            // Проверка на заполненность полей
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            // Проверка совпадения пароля
            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.");
                return;
            }

            // Проверка формата пароля
            if (!IsPasswordValid(password))
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов, только английские буквы и хотя бы одну цифру.");
                return;
            }

            // Проверка на существование пользователя с таким же логином
            if (db.users.Any(u => u.username == login)) // Используем таблицу User
            {
                MessageBox.Show("Пользователь с таким логином уже существует.");
                return;
            }

            try
            {
                
                var newUser = new user // Объект для таблицы User
                {
                    username = login,
                    password_hash = GetHash(password), // Хешируем пароль перед сохранением
                    role = role,
                    created_at = DateTime.Now,

                };
                if (newUser.user_id == 0)
                    db.users.Add(newUser);// Добавление записи в таблицу User
                try
                {
                    db.SaveChanges();
                    MessageBox.Show("Данные успешно сохранены!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

                NavigationService?.Navigate(new AuthPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}");
            }
        }

        // Метод для проверки формата пароля
        private bool IsPasswordValid(string password)
        {
            if (password.Length < 6)return false;

            if (!Regex.IsMatch(password, @"^[a-zA-Z0-9]+$"))return false;

            if (!Regex.IsMatch(password, @"\d"))return false;
            return true;
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
