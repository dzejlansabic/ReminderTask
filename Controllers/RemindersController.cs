using Microsoft.AspNetCore.Mvc;

namespace ReminderTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemindersController : ControllerBase
    {
        // GET: api/<RemindersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<RemindersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<RemindersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<RemindersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RemindersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
