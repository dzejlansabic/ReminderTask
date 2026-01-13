using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReminderTask.Application.DTOs;
using ReminderTask.Infrastructure.Data;
using ReminderTask.Models;

namespace ReminderTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemindersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public RemindersController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/<RemindersController>
        [HttpGet]
        public async Task<IEnumerable<ReminderResponse>> Get()
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
                        .ToListAsync();
        }

        // GET api/<RemindersController>/e6495f66-b693-4a14-8b5d-287caeff60e7
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid reminder ID.");

            var reminder = await _db.Reminders.FindAsync(id);

            if (reminder == null)
                return NotFound();

            return Ok(new ReminderResponse
            {
                Id = reminder.Id,
                Message = reminder.Message,
                SendAt = reminder.SendAt,
                Status = reminder.Status.ToString()
            });
        }

        // POST api/<RemindersController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReminderCreateRequest request)
        {
            if (request.SendAt <= DateTime.UtcNow)
            {
                return BadRequest("SendAt must be in the future.");
            }

            var reminder = new Reminder
            {
                Message = request.Message.Trim(),
                SendAt = request.SendAt.UtcDateTime,
                Email = request.Email
            };

            _db.Reminders.Add(reminder);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                Id = reminder.Id,
                Status = reminder.Status.ToString(),
                SendAt = reminder.SendAt
            });
        }

        // PUT api/<RemindersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] ReminderUpdateRequest request)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid reminder ID.");

            if (request.SendAt <= DateTime.UtcNow)
                return BadRequest("SendAt must be in the future.");

            var reminder = await _db.Reminders.FindAsync(id);

            if (reminder == null)
                return NotFound();

            reminder.Message = request.Message.Trim();
            reminder.SendAt = request.SendAt.UtcDateTime;
            reminder.Email = request.Email;

            await _db.SaveChangesAsync();

            return Ok(new ReminderResponse
            {
                Id = reminder.Id,
                Message = reminder.Message,
                SendAt = reminder.SendAt,
                Status = reminder.Status.ToString()
            });
        }

        // DELETE api/<RemindersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid reminder ID.");

            var reminder = await _db.Reminders.FindAsync(id);

            if (reminder == null)
                return NotFound();

            _db.Reminders.Remove(reminder);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
