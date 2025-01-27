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

        private async void OnAddTaskClicked(object sender, EventArgs e)
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
                    NotificationTime = notificationDateTime, // Используем NotificationTime для хранения времени
                    IsCompleted = false,
                    StartTime = DateTime.Now // Устанавливаем текущее время как время создания задачи
                };

                _tasks.Add(newTask); // Добавляем новую задачу в список

                // Возвращаемся на главную страницу
                await Navigation.PopAsync();
            }
            else
            {
                // Показать сообщение об ошибке, если название задачи пустое
                await DisplayAlert("Ошибка", "Пожалуйста, введите название задачи.", "OK");
            }
        }
    }
}
