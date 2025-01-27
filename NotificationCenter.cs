using Plugin.LocalNotification;
using System;
using System.Threading.Tasks;

namespace SchedulePlannerApp
{
    internal class NotificationCenter
    {
        /// <summary>
        /// Отправляет уведомление на устройство.
        /// </summary>
        /// <param name="taskName">Название задачи.</param>
        /// <param name="notificationTime">Время, когда нужно отправить уведомление.</param>
        public static async Task ScheduleNotification(string taskName, DateTime notificationTime)
        {
            // Если уведомление нужно отправить в прошлом, игнорируем.
            if (notificationTime <= DateTime.Now)
                return;

            var notification = new NotificationRequest
            {
                NotificationId = GenerateNotificationId(),
                Title = "Напоминание о задаче",
                Description = taskName,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notificationTime // Указываем время уведомления
                }
            };

            // Планируем уведомление
            await LocalNotificationCenter.Current.Show(notification);
        }

        /// <summary>
        /// Отменяет уведомление по ID задачи.
        /// </summary>
        /// <param name="notificationId">ID уведомления.</param>
        public static void CancelNotification(int notificationId)
        {
            LocalNotificationCenter.Current.Cancel(notificationId);
        }

        /// <summary>
        /// Генерирует уникальный ID для уведомлений.
        /// </summary>
        /// <returns>Уникальный ID.</returns>
        private static int GenerateNotificationId()
        {
            return new Random().Next(1, int.MaxValue);
        }
    }
}
