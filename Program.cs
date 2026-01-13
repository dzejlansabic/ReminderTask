using Microsoft.EntityFrameworkCore;
using ReminderTask.Application.Notifications;
using ReminderTask.Application.Notifications.Senders;
using ReminderTask.Application.Services;
using ReminderTask.Configuration;
using ReminderTask.Infrastructure.BackgroundServices;
using ReminderTask.Infrastructure.Data;
using ReminderTask.Infrastructure.Email;
using ReminderTask.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));


builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();
builder.Services.AddSingleton<INotificationSender, EmailNotificationSender>();
builder.Services.AddSingleton<INotificationSender, ConsoleNotificationSender>();
builder.Services.AddScoped<IReminderService, ReminderService>();

builder.Services.AddHostedService<ReminderWorker>();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
