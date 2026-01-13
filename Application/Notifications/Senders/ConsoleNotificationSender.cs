using ReminderTask.Application.Notifications;
using ReminderTask.Models;

namespace ReminderTask.Application.Notifications.Senders
{
    public class ConsoleNotificationSender : INotificationSender
    {
        private readonly ILogger<ConsoleNotificationSender> _logger;

        public ConsoleNotificationSender(ILogger<ConsoleNotificationSender> logger)
        {
            _logger = logger;
        }

        public bool CanSend(Reminder reminder) => string.IsNullOrWhiteSpace(reminder.Email);

        public Task SendAsync(Reminder reminder, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "[{Timestamp}] Reminder sent: {Message}",
                DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                reminder.Message);
            return Task.CompletedTask;
        }
    }
}
