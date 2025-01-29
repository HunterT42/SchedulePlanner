using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Text.Json;

namespace SchedulePlannerApp
{
    public partial class AddTaskPage : ContentPage
    {
        private ObservableCollection<TaskItem> _tasks; // Коллекция задач
        private ObservableCollection<string> _categories; // Коллекция категорий

        public AddTaskPage(ObservableCollection<TaskItem> tasks)
        {
            InitializeComponent();
            _tasks = tasks;

            // Загружаем категории из памяти
            _categories = LoadCategories();

            if (CategoryPicker != null) // Проверяем, что CategoryPicker существует
            {
                CategoryPicker.ItemsSource = _categories;

                if (_categories.Count > 0)
                    CategoryPicker.SelectedIndex = 0; // Выбираем первую категорию по умолчанию
            }
        }

        // Добавление новой задачи
        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            var taskName = TaskNameEntry.Text;
            var selectedDate = TaskDatePicker.Date;
            var selectedTime = TaskTimePicker.Time;
            var selectedCategory = CategoryPicker?.SelectedItem as string; // Проверяем, что CategoryPicker не null

            var notificationDateTime = selectedDate + selectedTime; // Объединяем дату и время

            if (!string.IsNullOrWhiteSpace(taskName) && !string.IsNullOrWhiteSpace(selectedCategory))
            {
                var newTask = new TaskItem
                {
                    Name = taskName,
                    Time = notificationDateTime.ToString("g"),
                    NotificationTime = notificationDateTime,
                    StartTime = DateTime.Now,
                    Category = selectedCategory
                };

                _tasks.Add(newTask);
                await Navigation.PopAsync(); // Возвращаемся на предыдущую страницу
            }
            else
            {
                await DisplayAlert("Ошибка", "Введите название задачи и выберите категорию.", "OK"); // Выводим сообщение об ошибке
            }
        }

        // Загрузка сохраненных категорий
        private ObservableCollection<string> LoadCategories()
        {
            if (Preferences.ContainsKey("TaskCategories")) // Проверяем, есть ли сохранённые категории
            {
                string json = Preferences.Get("TaskCategories", string.Empty);

                if (!string.IsNullOrWhiteSpace(json)) // Проверяем, что строка не пустая
                {
                    return JsonSerializer.Deserialize<ObservableCollection<string>>(json) ?? new ObservableCollection<string>();
                }
            }

            // Если данных нет, загружаем категории по умолчанию
            return new ObservableCollection<string> { "Здоровье", "Семья", "Личное" };
        }
    }
}



