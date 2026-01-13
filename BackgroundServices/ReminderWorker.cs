using Microsoft.EntityFrameworkCore;
using ReminderTask.Data;
using ReminderTask.Models;
using ReminderTask.Notifications;

namespace ReminderTask.BackgroundServices
{
    public class ReminderWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IEnumerable<INotificationSender> _senders;

        public ReminderWorker(
            IServiceScopeFactory scopeFactory,
            IEnumerable<INotificationSender> senders)
        {
            _scopeFactory = scopeFactory;
            _senders = senders;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Process(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private async Task Process(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var reminders = await db.Reminders
                .Where(r => r.Status == ReminderStatus.Scheduled &&
                            r.SendAt <= DateTime.UtcNow)
                .ToListAsync(ct);

            foreach (var reminder in reminders)
            {
                try
                {
                    var CorrectSenders = _senders.Where(s => s.CanSend(reminder)).ToList();

                    if (!CorrectSenders.Any())
                    {
                        reminder.Status = ReminderStatus.Failed;
                        continue;
                    }
                    foreach (var sender in CorrectSenders)
                    {
                        await sender.SendAsync(reminder);
                    }

                    reminder.Status = ReminderStatus.Sent;
                    reminder.SentAt = DateTime.UtcNow;
                }
                catch
                {
                    reminder.Status = ReminderStatus.Failed;
                }
            }

            if (reminders.Any())
                await db.SaveChangesAsync(ct);
        }
    }

}