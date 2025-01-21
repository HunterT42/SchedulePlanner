using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

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

        private void OnAddTaskClicked(object sender, EventArgs e)
        {
            var taskName = TaskNameEntry.Text;
            var selectedDate = TaskDatePicker.Date;
            var selectedTime = TaskTimePicker.Time;

            // Создаем полное DateTime для уведомления
            var notificationDateTime = selectedDate + selectedTime;

            if (!string.IsNullOrWhiteSpace(taskName))
            {
                var newTask = new TaskItem
                {
                    Name = taskName,
                    Time = notificationDateTime.ToString("g"),
                    NotificationTime = notificationDateTime,
                    IsCompleted = false
                };

                _tasks.Add(newTask); // Добавляем новую задачу в список
                Navigation.PopAsync(); // Возвращаемся на главную страницу
            }
        }
    }
}
