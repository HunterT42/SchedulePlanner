using System.Collections.ObjectModel;
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
    }
}
