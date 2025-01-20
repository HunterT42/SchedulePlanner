using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using static SchedulePlannerApp.MainPage;

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
                Time = TaskTimePicker.Time.ToString(),
                NotificationTime = DateTime.Today.Add(TaskTimePicker.Time) // Установка времени уведомления на основе выбранного времени
            };

            _tasks.Add(task);
            await Navigation.PopAsync();
        }

    }
}
