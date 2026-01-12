namespace ReminderTask.Configuration
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = null!;
        public int SmtpPort { get; set; } = 587;
        public string FromEmail { get; set; } = null!;
        public string FromName { get; set; } = "Reminder Service";
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool UseSsl { get; set; } = true;
    }
}
