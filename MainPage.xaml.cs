using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace SchedulePlannerApp
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<TaskItem> Tasks { get; set; }
        public ObservableCollection<TaskItem> CompletedTasks { get; set; }

        public MainPage()
        {
            InitializeComponent();
            Tasks = new ObservableCollection<TaskItem>();
            CompletedTasks = new ObservableCollection<TaskItem>();
            TaskListView.ItemsSource = Tasks;
            Tasks.CollectionChanged += (s, e) => SaveTasks();
            CompletedTasks.CollectionChanged += (s, e) => SaveCompletedTasks();
            LoadTasks();
            LoadCompletedTasks();
            StartTimer(); // Запускаем таймер для обновления оставшегося времени
        }

        private void StartTimer()
        {
            Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                foreach (var task in Tasks)
                {
                    OnPropertyChanged(nameof(task.TimeRemaining));
                }
                return true; // Таймер продолжается
            });
        }

        private void SaveTasks()
        {
            var tasksJson = JsonSerializer.Serialize(Tasks);
            Preferences.Set("SavedTasks", tasksJson);
        }

        private void SaveCompletedTasks()
        {
            var completedTasksJson = JsonSerializer.Serialize(CompletedTasks);
            Preferences.Set("SavedCompletedTasks", completedTasksJson);
        }

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
            await Navigation.PushAsync(new AddTaskPage(Tasks));
        }

        private void OnDeleteTaskClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button?.CommandParameter is TaskItem task)
            {
                Tasks.Remove(task);
                SaveTasks();
            }
        }

        private void OnCompleteTaskClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button?.CommandParameter is TaskItem task)
            {
                task.IsCompleted = true;
                task.StartTime = DateTime.Now;
                CompletedTasks.Add(task);
                Tasks.Remove(task);
                SaveTasks();
                SaveCompletedTasks();
            }
        }

        private async void OnViewCompletedTasksClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CompletedTasksPage(CompletedTasks));
        }

        private async void OnExportTasksClicked(object sender, EventArgs e)
        {
            try
            {
                var tasksJson = JsonSerializer.Serialize(Tasks);
                var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, "tasks.json");
                File.WriteAllText(filePath, tasksJson);

                await DisplayAlert("Экспорт завершен", $"Задачи экспортированы в файл:\n{filePath}", "ОК");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка экспорта", $"Не удалось экспортировать задачи: {ex.Message}", "ОК");
            }
        }

        private async void OnImportTasksClicked(object sender, EventArgs e)
        {
            try
            {
                var customJsonFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.json" } },
                    { DevicePlatform.Android, new[] { "application/json" } },
                    { DevicePlatform.WinUI, new[] { ".json" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.json" } }
                });

                var pickOptions = new PickOptions
                {
                    FileTypes = customJsonFileType,
                    PickerTitle = "Выберите JSON файл для импорта"
                };

                var fileResult = await FilePicker.Default.PickAsync(pickOptions);

                if (fileResult != null)
                {
                    var importedJson = File.ReadAllText(fileResult.FullPath);
                    var importedTasks = JsonSerializer.Deserialize<ObservableCollection<TaskItem>>(importedJson);

                    if (importedTasks != null)
                    {
                        foreach (var task in importedTasks)
                        {
                            Tasks.Add(task);
                        }
                        SaveTasks();
                        await DisplayAlert("Импорт завершен", "Задачи успешно импортированы.", "ОК");
                    }
                    else
                    {
                        await DisplayAlert("Ошибка импорта", "Файл не содержит корректные задачи.", "ОК");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка импорта", $"Не удалось импортировать задачи: {ex.Message}", "ОК");
            }
        }
    }

    public class TaskItem
    {
        public string Name { get; set; }
        public string Time { get; set; }
        public DateTime NotificationTime { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime StartTime { get; set; }

        public string TimeRemaining
        {
            get
            {
                var remaining = NotificationTime - DateTime.Now;
                if (remaining > TimeSpan.Zero)
                {
                    return $"{remaining.Days}д {remaining:hh\\:mm}";
                }
                return "Время истекло";
            }
        }

        public TimeSpan? Duration
        {
            get
            {
                if (IsCompleted)
                {
                    return DateTime.Now - StartTime;
                }
                return null;
            }
        }
    }
}
