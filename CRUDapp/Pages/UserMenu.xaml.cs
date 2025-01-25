using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace CRUDapp.Pages
{
    public partial class UserMenu : Page
    {
        private int _userId; 
        private ToDoListEntities db = new ToDoListEntities(); 

        
        public UserMenu(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadTasks();
            this.Loaded += UserMenu_Loaded;
        }
        private void UserMenu_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTasks(); // Обновляем данные при загрузке страницы
        }
        // Метод для загрузки задач пользователя
        private void LoadTasks()
        {
            try
            {
                // Получаем задачи текущего пользователя из базы данных
                var tasks = db.tasks
                    .Where(t => t.user_id == _userId)
                    .Select(t => new
                    {
                        t.task_id, // Убедимся, что это свойство есть
                        t.task_name,
                        t.description,
                        priority = t.priority.priority_name, // Название приоритета
                        status = t.status.status_name       // Название статуса
                    })
                    .ToList();

                // Привязка данных к DataGrid
                TasksDataGrid.ItemsSource = tasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке задач: " + ex.Message);
            }
        }


        private void TasksDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedTask = TasksDataGrid.SelectedItem as dynamic;
            if (selectedTask != null)
            {
                NavigationService?.Navigate(new TaskPage(selectedTask.task_id, _userId));

                LoadTasks();
            }
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new TaskPage(_userId));

            LoadTasks();
        }

        private void DelTaskButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем выбранную задачу
                var selectedTask = TasksDataGrid.SelectedItem as dynamic;
                if (selectedTask != null)
                {
                    // Получаем task_id выбранной задачи
                    int taskId = selectedTask.task_id;

                    // Находим задачу в базе данных
                    var taskToDelete = db.tasks.FirstOrDefault(t => t.task_id == taskId);
                    if (taskToDelete != null)
                    {
                        // Удаляем задачу из базы данных
                        db.tasks.Remove(taskToDelete);
                        db.SaveChanges();

                        // Обновляем DataGrid
                        LoadTasks();

                        MessageBox.Show("Задача успешно удалена!");
                    }
                }
                else
                {
                    MessageBox.Show("Выберите задачу для удаления!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении задачи: " + ex.Message);
            }
        }
    }
}