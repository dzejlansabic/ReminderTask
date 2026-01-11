namespace ReminderTask.Models
{
    public class Reminder
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Message { get; set; } = null!;
        public DateTime SendAt { get; set; }
        public string? Email { get; set; }
        public ReminderStatus Status { get; set; } = ReminderStatus.Scheduled;
        public DateTime? SentAt { get; set; }
    }
}
