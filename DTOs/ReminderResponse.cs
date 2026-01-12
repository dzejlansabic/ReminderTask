namespace ReminderTask.DTOs
{
    public class ReminderResponse
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = null!;
        public DateTime SendAt { get; set; }
        public string Status { get; set; } = null!;
    }
}
