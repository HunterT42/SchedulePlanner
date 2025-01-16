using System.Collections.ObjectModel;

namespace SchedulePlannerApp
{
    public partial class MainPage : ContentPage
    {
        // Коллекция для хранения задач
        public ObservableCollection<TaskItem> Tasks { get; set; }

        public MainPage()
        {
            InitializeComponent(); // Связывает XAML с кодом

            // Инициализация коллекции задач
            Tasks = new ObservableCollection<TaskItem>();
            TaskListView.ItemsSource = Tasks; // Привязка списка к интерфейсу
        }

        // Обработчик кнопки добавления задачи
        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            // Переход на страницу добавления задачи
            await Navigation.PushAsync(new AddTaskPage(Tasks));
        }
    }

    // Модель данных для задачи
    public class TaskItem
    {
        public string Name { get; set; } // Название задачи
        public string Time { get; set; } // Время напоминания
    }
}
