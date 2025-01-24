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
            Preferences.Set("SavedCompletedTasks", completedTasksJson);
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
            await Navigation.PushAsync(new AddTaskPage(Tasks));
        }

        private void OnDeleteTaskClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            if (button?.CommandParameter is TaskItem task)
            {
                Tasks.Remove(task);
                SaveTasks();
            }
        }

        // Пометка задачи как выполненной
        private void OnCompleteTaskClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            if (button?.CommandParameter is TaskItem task)
            {
                task.IsCompleted = true;
                CompletedTasks.Add(task);
                Tasks.Remove(task);
                SaveTasks();
            }
        }

        private async void OnViewCompletedTasksClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CompletedTasksPage(CompletedTasks));
        }

        // Экспорт задач в JSON-файл
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

        // Импорт задач из JSON-файла
        private async void OnImportTasksClicked(object sender, EventArgs e)
        {
            try
            {
                // Определяем пользовательский тип файла JSON
                var customJsonFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.iOS, new[] { "public.json" } }, // iOS
            { DevicePlatform.Android, new[] { "application/json" } }, // Android
            { DevicePlatform.WinUI, new[] { ".json" } }, // Windows
            { DevicePlatform.MacCatalyst, new[] { "public.json" } } // Mac
        });

                // Настраиваем параметры выбора файла
                var pickOptions = new PickOptions
                {
                    FileTypes = customJsonFileType,
                    PickerTitle = "Выберите JSON файл для импорта"
                };

                // Вызываем диалог выбора файла
                var fileResult = await FilePicker.Default.PickAsync(pickOptions);

                if (fileResult != null)
                {
                    // Читаем содержимое выбранного файла
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

    // Модель данных для задачи
    public class TaskItem
    {
        public string Name { get; set; }
        public string Time { get; set; }
        public DateTime NotificationTime { get; set; }
        public bool IsCompleted { get; set; }
    }
}
