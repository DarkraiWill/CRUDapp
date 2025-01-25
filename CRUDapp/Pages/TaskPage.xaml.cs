using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CRUDapp.Pages
{
    public partial class TaskPage : Page
    {
        private int _userId; // Идентификатор пользователя
        private int? _taskId; // Идентификатор задачи (null, если это добавление новой задачи)
        private ToDoListEntities db = new ToDoListEntities(); // Контекст базы данных

        // Конструктор для добавления новой задачи
        public TaskPage(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _taskId = null; // Режим добавления новой задачи
            LoadPrioritiesAndStatuses();
        }

        // Конструктор для редактирования существующей задачи
        public TaskPage(int taskId, int userId)
        {
            InitializeComponent();
            _userId = userId;
            _taskId = taskId; // Режим редактирования задачи
            LoadPrioritiesAndStatuses();
            LoadTaskData();
        }

        // Загрузка приоритетов и статусов из базы данных
        private void LoadPrioritiesAndStatuses()
        {
            try
            {
                // Загрузка приоритетов
                PriorityComboBox.ItemsSource = db.priorities.ToList();
                PriorityComboBox.SelectedValuePath = "priority_id"; // Указываем, что SelectedValue будет равен priority_id

                // Загрузка статусов
                StatusComboBox.ItemsSource = db.status.ToList();
                StatusComboBox.SelectedValuePath = "status_id"; // Указываем, что SelectedValue будет равен status_id
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        // Загрузка данных задачи для редактирования
        private void LoadTaskData()
        {
            try
            {
                if (_taskId.HasValue)
                {
                    var task = db.tasks.FirstOrDefault(t => t.task_id == _taskId);
                    if (task != null)
                    {
                        TaskNameTextBox.Text = task.task_name;
                        DescriptionTextBox.Text = task.description;
                        PriorityComboBox.SelectedValue = task.priority_id; // Устанавливаем SelectedValue для ComboBox
                        StatusComboBox.SelectedValue = task.status_id;    // Устанавливаем SelectedValue для ComboBox
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных задачи: " + ex.Message);
            }
        }

        // Обработчик сохранения задачи
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_taskId.HasValue)
                {
                    // Режим редактирования задачи
                    var task = db.tasks.FirstOrDefault(t => t.task_id == _taskId);
                    if (task != null)
                    {
                        task.task_name = TaskNameTextBox.Text;
                        task.description = DescriptionTextBox.Text;
                        task.priority_id = (int)PriorityComboBox.SelectedValue; // Приоритет
                        task.status_id = (int)StatusComboBox.SelectedValue;     // Статус
                        task.due_date = DateTime.Now;
                    }
                }
                else
                {
                    // Режим добавления новой задачи
                    var newTask = new task
                    {
                        task_name = TaskNameTextBox.Text,
                        description = DescriptionTextBox.Text,
                        user_id = _userId,
                        priority_id = (int)PriorityComboBox.SelectedValue, // Приоритет
                        status_id = (int)StatusComboBox.SelectedValue,     // Статус
                        created_at = DateTime.Now
                    };
                    db.tasks.Add(newTask);
                }

                db.SaveChanges();
                MessageBox.Show("Задача успешно сохранена!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении задачи: " + ex.Message);
            }
        }
    }
}
