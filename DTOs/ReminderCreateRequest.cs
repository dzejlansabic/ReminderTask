using System.ComponentModel.DataAnnotations;

namespace ReminderTask.DTOs
{
    public class ReminderCreateRequest
    {
        [Required]
        [MinLength(1)]
        [MaxLength(500)]
        public string Message { get; set; } = null!;

        [Required]
        public DateTime SendAt { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}
