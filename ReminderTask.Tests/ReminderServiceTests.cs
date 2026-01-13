using Microsoft.EntityFrameworkCore;
using ReminderTask.Application.DTOs;
using ReminderTask.Application.Notifications;
using ReminderTask.Application.Services;
using ReminderTask.Infrastructure.Data;
using ReminderTask.Models;
using Xunit;

namespace ReminderTask.Tests
{
    public class ReminderServiceTests
    {
        private static AppDbContext CreateInMemoryDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOrderedReminders()
        {
            using var db = CreateInMemoryDb("GetAllDb");
            db.Reminders.Add(new Reminder { Message = "B", SendAt = DateTime.UtcNow.AddHours(2) });
            db.Reminders.Add(new Reminder { Message = "A", SendAt = DateTime.UtcNow.AddHours(1) });
            await db.SaveChangesAsync();

            var service = new ReminderService(db, Enumerable.Empty<INotificationSender>());

            var results = await service.GetAllAsync();

            Assert.Equal(2, results.Count);
            Assert.Equal("A", results[0].Message);
            Assert.Equal("B", results[1].Message);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsReminder_WhenExists()
        {
            using var db = CreateInMemoryDb("GetByIdDb");
            var reminder = new Reminder { Message = "Test", SendAt = DateTime.UtcNow.AddHours(1) };
            db.Reminders.Add(reminder);
            await db.SaveChangesAsync();

            var service = new ReminderService(db, Enumerable.Empty<INotificationSender>());

            var result = await service.GetByIdAsync(reminder.Id);

            Assert.NotNull(result);
            Assert.Equal(reminder.Id, result!.Id);
            Assert.Equal(reminder.Message, result.Message);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            using var db = CreateInMemoryDb("GetByIdNotFoundDb");
            var service = new ReminderService(db, Enumerable.Empty<INotificationSender>());

            var result = await service.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_CreatesReminder()
        {
            using var db = CreateInMemoryDb("CreateDb");
            var service = new ReminderService(db, Enumerable.Empty<INotificationSender>());

            var request = new ReminderCreateRequest
            {
                Message = "  Hello ",
                SendAt = DateTimeOffset.UtcNow.AddHours(1),
                Email = "test@example.com"
            };

            var result = await service.CreateAsync(request);

            Assert.NotNull(result);
            Assert.Equal("Hello", result.Message);
            Assert.Equal(request.SendAt.UtcDateTime, result.SendAt);
            Assert.Equal(request.Email, db.Reminders.Single().Email);
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenSendAtInPast()
        {
            using var db = CreateInMemoryDb("CreatePastDb");
            var service = new ReminderService(db, Enumerable.Empty<INotificationSender>());

            var request = new ReminderCreateRequest
            {
                Message = "Hi",
                SendAt = DateTimeOffset.UtcNow.AddHours(-1)
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(request));
        }

        [Fact]
        public async Task UpdateAsync_UpdatesReminder()
        {
            using var db = CreateInMemoryDb("UpdateDb");
            var reminder = new Reminder { Message = "Old", SendAt = DateTime.UtcNow.AddHours(1), Email = "old@example.com" };
            db.Reminders.Add(reminder);
            await db.SaveChangesAsync();

            var service = new ReminderService(db, Enumerable.Empty<INotificationSender>());

            var request = new ReminderUpdateRequest
            {
                Message = " New ",
                SendAt = DateTimeOffset.UtcNow.AddHours(2),
                Email = "new@example.com"
            };

            var result = await service.UpdateAsync(reminder.Id, request);

            Assert.Equal("New", result.Message);
            Assert.Equal(request.SendAt.UtcDateTime, result.SendAt);
            Assert.Equal(request.Email, db.Reminders.Single(r => r.Id == reminder.Id).Email);
        }

        [Fact]
        public async Task UpdateAsync_Throws_WhenIdEmpty()
        {
            using var db = CreateInMemoryDb("UpdateEmptyIdDb");
            var service = new ReminderService(db, Enumerable.Empty<INotificationSender>());

            var request = new ReminderUpdateRequest
            {
                Message = "x",
                SendAt = DateTimeOffset.UtcNow.AddHours(1)
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateAsync(Guid.Empty, request));
        }

        [Fact]
        public async Task UpdateAsync_Throws_WhenSendAtInPast()
        {
            using var db = CreateInMemoryDb("UpdatePastDb");
            var reminder = new Reminder { Message = "Old", SendAt = DateTime.UtcNow.AddHours(1) };
            db.Reminders.Add(reminder);
            await db.SaveChangesAsync();

            var service = new ReminderService(db, Enumerable.Empty<INotificationSender>());

            var request = new ReminderUpdateRequest
            {
                Message = "x",
                SendAt = DateTimeOffset.UtcNow.AddHours(-1)
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateAsync(reminder.Id, request));
        }

        [Fact]
        public async Task UpdateAsync_Throws_WhenNotFound()
        {
            using var db = CreateInMemoryDb("UpdateNotFoundDb");
            var service = new ReminderService(db, Enumerable.Empty<INotificationSender>());

            var request = new ReminderUpdateRequest
            {
                Message = "x",
                SendAt = DateTimeOffset.UtcNow.AddHours(1)
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateAsync(Guid.NewGuid(), request));
        }

        [Fact]
        public async Task DeleteAsync_DeletesReminder_WhenExists()
        {
            using var db = CreateInMemoryDb("DeleteDb");
            var reminder = new Reminder { Message = "Delete", SendAt = DateTime.UtcNow.AddHours(1) };
            db.Reminders.Add(reminder);
            await db.SaveChangesAsync();

            var service = new ReminderService(db, Enumerable.Empty<INotificationSender>());

            var result = await service.DeleteAsync(reminder.Id);

            Assert.True(result);
            Assert.Empty(db.Reminders.ToList());
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
        {
            using var db = CreateInMemoryDb("DeleteNotFoundDb");
            var service = new ReminderService(db, Enumerable.Empty<INotificationSender>());

            var result = await service.DeleteAsync(Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_Throws_WhenIdEmpty()
        {
            using var db = CreateInMemoryDb("DeleteEmptyIdDb");
            var service = new ReminderService(db, Enumerable.Empty<INotificationSender>());

            await Assert.ThrowsAsync<ArgumentException>(() => service.DeleteAsync(Guid.Empty));
        }
    }
}
