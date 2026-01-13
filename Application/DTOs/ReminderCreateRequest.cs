using System.ComponentModel.DataAnnotations;

namespace ReminderTask.Application.DTOs
{
    public class ReminderCreateRequest
    {
        [Required]
        [MinLength(1)]
        [MaxLength(500)]
        public string Message { get; set; } = null!;

        [Required]
        public DateTimeOffset SendAt { get; set; }

        [EmailAddress]
        public string? Email { get; set; } = null;
    }
}
