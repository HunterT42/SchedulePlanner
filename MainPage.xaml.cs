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
        public ObservableCollection<string> Categories { get; set; }

        public MainPage()
        {
            InitializeComponent();
            Tasks = new ObservableCollection<TaskItem>();
            CompletedTasks = new ObservableCollection<TaskItem>();
            Categories = new ObservableCollection<string> { "Здоровье", "Семья", "Личное" }; // Базовые категории

            TaskListView.ItemsSource = Tasks;
            Tasks.CollectionChanged += (s, e) => SaveTasks();
            CompletedTasks.CollectionChanged += (s, e) =>
            {
                SaveCompletedTasks();
                UpdateStatistics(); // Обновляем статистику при изменении выполненных задач <<<
            };

            LoadTasks();
            LoadCompletedTasks();
            StartTimer();
        }

        private void StartTimer()
        {
            Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                foreach (var task in Tasks)
                {
                    OnPropertyChanged(nameof(task.TimeRemaining));
                }
                return true;
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
                    // Совместимость со старыми версиями <<<
                    if (task.IsCompleted && !task.EndTime.HasValue)
                    {
                        task.EndTime = task.StartTime.AddMinutes(1);
                    }
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
                task.EndTime = DateTime.Now; // Фиксация времени завершения <<<
                CompletedTasks.Add(task);
                Tasks.Remove(task);
                SaveTasks();
                SaveCompletedTasks();
                UpdateStatistics(); // Обновляем статистику после завершения задачи <<<
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

        private async void OnEditCategoriesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditCategoriesPage());
        }

        private void OnViewStatisticsClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new StatisticsPage(CompletedTasks)); // Передаём выполненные задачи
        }

        // Обновление статистики <<<
        private void UpdateStatistics()
        {
            Preferences.Set("CompletedTaskCount", CompletedTasks.Count);

            if (CompletedTasks.Count > 0)
            {
                var averageDuration = CompletedTasks
                    .Where(task => task.EndTime.HasValue)
                    .Average(task => (task.EndTime.Value - task.StartTime).TotalMinutes);

                Preferences.Set("AverageTaskDuration", Math.Round(averageDuration, 2));
            }
        }
    }

    public class TaskItem
    {
        public string Name { get; set; }
        public string Time { get; set; } // Указанное время выполнения
        public DateTime NotificationTime { get; set; } // Время для уведомления
        public bool IsCompleted { get; set; } // Статус выполнения
        public DateTime StartTime { get; set; } // Время создания задачи
        public DateTime? EndTime { get; set; } // Время завершения задачи
        public string Category { get; set; } // Категория задачи

        // Оставшееся время до выполнения задачи
        public string TimeRemaining
        {
            get
            {
                if (IsCompleted) return "Завершено";

                var remaining = NotificationTime - DateTime.Now;
                return remaining > TimeSpan.Zero ? remaining.ToString(@"d'д 'h'ч 'm'м'") : "Время истекло";
            }
        }

        // Продолжительность выполнения задачи
        public string DurationFormatted
        {
            get
            {
                if (IsCompleted && EndTime.HasValue)
                {
                    var duration = EndTime.Value - StartTime;
                    return duration.ToString(@"d'д 'h'ч 'm'м'");
                }
                return "Нет данных";
            }
        }
    }
}
