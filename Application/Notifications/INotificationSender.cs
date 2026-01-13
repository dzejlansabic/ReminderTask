using ReminderTask.Models;

namespace ReminderTask.Application.Notifications
{
    public interface INotificationSender
    {
        bool CanSend(Reminder reminder);
        Task SendAsync(Reminder reminder, CancellationToken cancellationToken = default);
    }
}
