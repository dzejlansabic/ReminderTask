using System.ComponentModel.DataAnnotations;

namespace ReminderTask.DTOs
{
    public class ReminderUpdateRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(500)]
        public string Message { get; set; } = null!;

        [Required]
        public DateTime SendAt { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}
