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

            // ������� ������ DateTime ��� �����������
            var notificationDateTime = selectedDate + selectedTime;

            if (!string.IsNullOrWhiteSpace(taskName))
            {
                var newTask = new TaskItem
                {
                    Name = taskName,
                    NotificationTime = notificationDateTime, // ���������� NotificationTime ��� �������� �������
                    IsCompleted = false,
                    StartTime = DateTime.Now // ������������� ������� ����� ��� ����� �������� ������
                };

                _tasks.Add(newTask); // ��������� ����� ������ � ������

                // ������������ �� ������� ��������
                await Navigation.PopAsync();
            }
            else
            {
                // �������� ��������� �� ������, ���� �������� ������ ������
                await DisplayAlert("������", "����������, ������� �������� ������.", "OK");
            }
        }
    }
}
