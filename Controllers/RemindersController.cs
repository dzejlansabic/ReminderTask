using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReminderTask.Application.DTOs;
using ReminderTask.Application.Services;
using ReminderTask.Infrastructure.Data;
using ReminderTask.Models;

namespace ReminderTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemindersController : ControllerBase
    {
        private readonly IReminderService _service;

        public RemindersController(IReminderService service)
        {
            _service = service;
        }

        // GET: api/<RemindersController>
        [HttpGet]
        public async Task<IEnumerable<ReminderResponse>> Get(CancellationToken ct)
        {
            return await _service.GetAllAsync(ct);
        }

        // GET api/<RemindersController>/e6495f66-b693-4a14-8b5d-287caeff60e7
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid reminder ID.");

            var reminder = await _service.GetByIdAsync(id, ct);

            if (reminder == null)
                return NotFound();

            return Ok(reminder);
        }

        // POST api/<RemindersController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReminderCreateRequest request, CancellationToken ct)
        {
            if (request.SendAt <= DateTime.UtcNow)
            {
                return BadRequest("SendAt must be in the future.");
            }

            try
            {
                var result = await _service.CreateAsync(request, ct);
                return Ok(new
                {
                    Id = result.Id,
                    Status = result.Status,
                    SendAt = result.SendAt
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<RemindersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] ReminderUpdateRequest request, CancellationToken ct)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid reminder ID.");

            if (request.SendAt <= DateTime.UtcNow)
                return BadRequest("SendAt must be in the future.");

            try
            {
                var updated = await _service.UpdateAsync(id, request, ct);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        // DELETE api/<RemindersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid reminder ID.");

            var deleted = await _service.DeleteAsync(id, ct);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}