using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace SchedulePlannerApp
{
    public partial class StatisticsPage : ContentPage
    {
        private ObservableCollection<TaskItem> _completedTasks;

        public StatisticsPage(ObservableCollection<TaskItem> completedTasks)
        {
            InitializeComponent(); // ОБЯЗАТЕЛЬНО, чтобы XAML загрузился!

            _completedTasks = completedTasks ?? new ObservableCollection<TaskItem>();
            CalculateStatistics();
        }

        private void CalculateStatistics()
        {
            // Если нет выполненных задач — обнуляем статистику
            if (_completedTasks == null || _completedTasks.Count == 0)
            {
                TotalTasksLabel.Text = "0";
                AverageDurationLabel.Text = "Нет данных";
                MostProductiveDayLabel.Text = "Нет данных";
                CategoryStatsList.ItemsSource = null;
                return;
            }

            // 1️⃣ Общее количество выполненных задач
            TotalTasksLabel.Text = _completedTasks.Count.ToString();

            // 2️⃣ Средняя длительность выполнения (в минутах)
            var completedWithEndTime = _completedTasks.Where(t => t.EndTime.HasValue).ToList();
            if (completedWithEndTime.Count > 0)
            {
                double averageDuration = completedWithEndTime
                    .Average(t => (t.EndTime.Value - t.StartTime).TotalMinutes); // УБРАН .Value У TimeSpan

                AverageDurationLabel.Text = Math.Round(averageDuration, 2).ToString();
            }
            else
            {
                AverageDurationLabel.Text = "Нет данных";
            }

            // 3️⃣ Самый продуктивный день (где больше всего задач)
            var mostProductiveDay = _completedTasks
                .Where(t => t.EndTime.HasValue)
                .GroupBy(t => t.EndTime.Value.Date)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            MostProductiveDayLabel.Text = mostProductiveDay?.Key.ToString("dd MMM yyyy") ?? "Нет данных";

            // 4️⃣ Считаем количество задач в каждой категории
            var categoryStats = _completedTasks
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToList();

            CategoryStatsList.ItemsSource = categoryStats;
        }
    }
}
