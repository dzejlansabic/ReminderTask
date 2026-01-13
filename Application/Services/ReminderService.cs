using Microsoft.EntityFrameworkCore;
using ReminderTask.Application.DTOs;
using ReminderTask.Application.Notifications;
using ReminderTask.Infrastructure.Data;
using ReminderTask.Models;

namespace ReminderTask.Application.Services
{
    public class ReminderService : IReminderService
    {
        private readonly AppDbContext _db;
        private readonly IEnumerable<INotificationSender> _senders;

        public ReminderService(AppDbContext db, IEnumerable<INotificationSender> senders)
        {
            _db = db;
            _senders = senders;
        }

        public async Task<IReadOnlyList<ReminderResponse>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Reminders
                .OrderBy(r => r.SendAt)
                .Select(r => new ReminderResponse
                {
                    Id = r.Id,
                    Message = r.Message,
                    SendAt = r.SendAt,
                    Status = r.Status.ToString()
                })
                .ToListAsync(ct);
        }

        public async Task<ReminderResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var reminder = await _db.Reminders.FindAsync(new object?[] { id }, ct);
            return reminder == null ? null : ToResponse(reminder);
        }

        public async Task<ReminderResponse> CreateAsync(ReminderCreateRequest request, CancellationToken ct = default)
        {
            if (request.SendAt <= DateTime.UtcNow)
                throw new ArgumentException("SendAt must be in the future.", nameof(request.SendAt));

            var reminder = new Reminder
            {
                Message = request.Message.Trim(),
                SendAt = request.SendAt.UtcDateTime,
                Email = request.Email
            };

            _db.Reminders.Add(reminder);
            await _db.SaveChangesAsync(ct);

            return ToResponse(reminder);
        }

        public async Task<ReminderResponse> UpdateAsync(Guid id, ReminderUpdateRequest request, CancellationToken ct = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid reminder ID.", nameof(id));

            if (request.SendAt <= DateTime.UtcNow)
                throw new ArgumentException("SendAt must be in the future.", nameof(request.SendAt));

            var reminder = await _db.Reminders.FindAsync(new object?[] { id }, ct);
            if (reminder == null)
                throw new InvalidOperationException("Reminder not found.");

            reminder.Message = request.Message.Trim();
            reminder.SendAt = request.SendAt.UtcDateTime;
            reminder.Email = request.Email;

            await _db.SaveChangesAsync(ct);

            return ToResponse(reminder);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid reminder ID.", nameof(id));

            var reminder = await _db.Reminders.FindAsync(new object?[] { id }, ct);

            if (reminder == null)
                return false;

            _db.Reminders.Remove(reminder);
            await _db.SaveChangesAsync(ct);

            return true;
        }

        private static ReminderResponse ToResponse(Reminder reminder)
        {
            return new ReminderResponse
            {
                Id = reminder.Id,
                Message = reminder.Message,
                SendAt = reminder.SendAt,
                Status = reminder.Status.ToString()
            };
        }
    }
}
