using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Plugin.LocalNotification;

namespace SchedulePlannerApp
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<TaskItem> Tasks { get; set; }

        public MainPage()
        {
            InitializeComponent();
            Tasks = new ObservableCollection<TaskItem>();
            TaskListView.ItemsSource = Tasks;
            Tasks.CollectionChanged += (s, e) => SaveTasks(); // Автосохранение при изменении списка задач
            LoadTasks(); // Загрузка задач при запуске приложения
        }

        // Сохранение задач в локальное хранилище
        private void SaveTasks()
        {
            var tasksJson = JsonSerializer.Serialize(Tasks); // Сериализация списка задач в JSON
            Preferences.Set("SavedTasks", tasksJson); // Сохранение JSON в локальном хранилище
        }

        // Загрузка задач из локального хранилища
        private void LoadTasks()
        {
            var tasksJson = Preferences.Get("SavedTasks", string.Empty); // Получение JSON из локального хранилища
            if (!string.IsNullOrEmpty(tasksJson))
            {
                var loadedTasks = JsonSerializer.Deserialize<ObservableCollection<TaskItem>>(tasksJson); // Десериализация JSON в список задач
                foreach (var task in loadedTasks)
                {
                    Tasks.Add(task); // Добавление задач в ObservableCollection
                    ScheduleNotification(task); // Планирование уведомления для каждой загруженной задачи
                }
            }
        }

        // Переход на страницу добавления новой задачи
        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTaskPage(Tasks)); // Переход на страницу AddTaskPage
        }

        // Удаление задачи из списка
        private void OnDeleteTaskClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            if (button?.CommandParameter is TaskItem task)
            {
                Tasks.Remove(task); // Удаление задачи из коллекции
            }
        }

        // Планирование локального уведомления
        private void ScheduleNotification(TaskItem task)
        {
            var timeUntilNotification = task.NotificationTime - DateTime.Now;

            // Логирование для проверки
            Console.WriteLine($"Scheduling notification for: {task.NotificationTime}, current time: {DateTime.Now}, time until notification: {timeUntilNotification}");

            if (timeUntilNotification > TimeSpan.Zero)
            {
                var notification = new LocalNotification
                {
                    Title = task.Name, // Заголовок уведомления
                    Body = task.Name, // Текст уведомления
                    Schedule =
            {
                NotifyTime = task.NotificationTime // Время уведомления
            }
                };

                LocalNotificationCenter.Current.Show(notification);
            }
        }



        // Модель данных для задачи
        public class TaskItem
        {
            public string Name { get; set; } // Название задачи
            public string Time { get; set; } // Время задачи в строковом формате
            public DateTime NotificationTime { get; set; } // Время для уведомления
        }
    }
}
