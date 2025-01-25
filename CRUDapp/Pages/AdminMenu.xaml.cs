using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CRUDapp.Pages
{
    public partial class AdminMenu : Page
    {
        private ToDoListEntities db = new ToDoListEntities();
        private string currentFilterText = string.Empty;
        private string currentRoleFilter = "Все";

        public AdminMenu()
        {
            InitializeComponent();
            LoadData();
        }
        // Метод для загрузки данных с применением фильтров
        private void LoadData()
        {
            try
            {
                var users = db.users.ToList();

                if (!string.IsNullOrEmpty(currentFilterText))
                {
                    users = users.Where(u => u.username.Contains(currentFilterText)).ToList();
                }

                if (currentRoleFilter != "Все")
                {
                    users = users.Where(u => u.role == currentRoleFilter).ToList();
                }


                DataGridUser.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }


        // Обработчик изменения текста в TextBox для фильтрации
        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentFilterText = FilterTextBox.Text;
            LoadData();
        }

        // Обработчик изменения выбора роли в ComboBox
        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = RoleComboBox.SelectedItem as ComboBoxItem;
            currentRoleFilter = selectedItem?.Content.ToString() ?? "Все";
            LoadData();
        }

        // Обработчик кнопки для применения фильтров
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        // Обработчик кнопки очистки фильтров
        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            FilterTextBox.Clear();
            RoleComboBox.SelectedIndex = 0;
            currentFilterText = string.Empty;
            currentRoleFilter = "Все";
            LoadData();
        }
    }
}
