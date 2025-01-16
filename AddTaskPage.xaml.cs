using System.Collections.ObjectModel;

namespace SchedulePlannerApp
{
    public partial class AddTaskPage : ContentPage
    {
        private ObservableCollection<TaskItem> _tasks;

        public AddTaskPage(ObservableCollection<TaskItem> tasks)
        {
            InitializeComponent();
            _tasks = tasks;
        }

        private async void OnSaveTaskClicked(object sender, EventArgs e)
        {
            var task = new TaskItem
            {
                Name = TaskNameEntry.Text,
                Time = TaskTimePicker.Time.ToString()
            };

            _tasks.Add(task);
            await Navigation.PopAsync();
        }
    }
}