using Microsoft.EntityFrameworkCore;
using ReminderTask.Models;

namespace ReminderTask.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Reminder> Reminders => Set<Reminder>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
