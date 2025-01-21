using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Maui.Controls;




namespace SchedulePlannerApp
{
    public partial class CompletedTasksPage : ContentPage
    {
        public ObservableCollection<TaskItem> CompletedTasks { get; set; }

        public CompletedTasksPage(ObservableCollection<TaskItem> completedTasks)
        {
            InitializeComponent();
            CompletedTasks = completedTasks;
            CompletedTaskListView.ItemsSource = CompletedTasks;
        }

        // Обработчик для удаления выполненной задачи
        private void OnDeleteCompletedTaskClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            if (button?.CommandParameter is TaskItem task)
            {
                CompletedTasks.Remove(task); // Удаляем задачу из списка выполненных задач
                // Здесь можно добавить сохранение состояния, если требуется
            }
        }

    }
}
