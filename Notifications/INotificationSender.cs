using ReminderTask.Models;

namespace ReminderTask.Notifications
{
    public interface INotificationSender
    {
        bool CanSend(Reminder reminder);
        Task SendAsync(Reminder reminder);
    }
}
