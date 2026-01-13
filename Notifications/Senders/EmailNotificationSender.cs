using ReminderTask.Models;
using ReminderTask.Services;

namespace ReminderTask.Notifications.Senders
{
    public class EmailNotificationSender : INotificationSender
    {
        private readonly IEmailSender _emailSender;

        public EmailNotificationSender(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public bool CanSend(Reminder reminder)
        => !string.IsNullOrWhiteSpace(reminder.Email);

        public async Task SendAsync(Reminder reminder, CancellationToken cancellationToken = default)
        {
                await _emailSender.SendEmailAsync(
                reminder.Email!,
                "Reminder",
                reminder.Message
            );
        }
    }
}
