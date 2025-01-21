using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace SchedulePlannerApp
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<TaskItem> Tasks { get; set; }
        public ObservableCollection<TaskItem> CompletedTasks { get; set; } // Новый список для выполненных задач

        public MainPage()
        {
            InitializeComponent();
            Tasks = new ObservableCollection<TaskItem>();
            CompletedTasks = new ObservableCollection<TaskItem>(); // Инициализируем коллекцию выполненных задач
            TaskListView.ItemsSource = Tasks;
            Tasks.CollectionChanged += (s, e) => SaveTasks();
            CompletedTasks.CollectionChanged += (s, e) => SaveCompletedTasks(); // Сохраняем выполненные задачи
            LoadTasks();
            LoadCompletedTasks(); // Загрузка выполненных задач
        }

        // Сохранение задач в локальное хранилище
        private void SaveTasks()
        {
            var tasksJson = JsonSerializer.Serialize(Tasks);
            Preferences.Set("SavedTasks", tasksJson);
        }

        // Сохранение выполненных задач
        private void SaveCompletedTasks()
        {
            var completedTasksJson = JsonSerializer.Serialize(CompletedTasks);
            Preferences.Set("SavedCompletedTasks", completedTasksJson); // Сохраняем в отдельное поле
        }

        // Загрузка задач из локального хранилища
        private void LoadTasks()
        {
            var tasksJson = Preferences.Get("SavedTasks", string.Empty);
            if (!string.IsNullOrEmpty(tasksJson))
            {
                var loadedTasks = JsonSerializer.Deserialize<ObservableCollection<TaskItem>>(tasksJson);
                foreach (var task in loadedTasks)
                {
                    Tasks.Add(task);
                }
            }
        }

        // Загрузка выполненных задач
        private void LoadCompletedTasks()
        {
            var completedTasksJson = Preferences.Get("SavedCompletedTasks", string.Empty);
            if (!string.IsNullOrEmpty(completedTasksJson))
            {
                var loadedCompletedTasks = JsonSerializer.Deserialize<ObservableCollection<TaskItem>>(completedTasksJson);
                foreach (var task in loadedCompletedTasks)
                {
                    CompletedTasks.Add(task);
                }
            }
        }

        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTaskPage(Tasks)); // Переход на страницу добавления задачи
        }

        private void OnDeleteTaskClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            if (button?.CommandParameter is TaskItem task)
            {
                Tasks.Remove(task); // Удаление задачи из списка
                SaveTasks(); // Сохраняем изменения
            }
        }

        // Пометка задачи как выполненной
        private void OnCompleteTaskClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            if (button?.CommandParameter is TaskItem task)
            {
                task.IsCompleted = true;
                CompletedTasks.Add(task); // Перемещаем задачу в список выполненных
                Tasks.Remove(task); // Удаляем задачу из списка задач
                SaveTasks(); // Сохраняем изменения
            }
        }


        private async void OnViewCompletedTasksClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CompletedTasksPage(CompletedTasks)); // Переход на страницу выполненных задач
        }
    }

    // Модель данных для задачи
    public class TaskItem
    {
        public string Name { get; set; }
        public string Time { get; set; }
        public DateTime NotificationTime { get; set; }
        public bool IsCompleted { get; set; }
    }
}
