using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotsController : ControllerBase
    {

        private readonly ChatDbContext _db;
        public BotsController(ChatDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetBots()
        {
            var bots = await _db.Bots.ToListAsync();
            return Ok(bots);
        }

        [HttpPost]
        public async Task<IActionResult> AddBot([FromBody] BotRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Context))
            {
                return BadRequest("Name and context cannot be empty.");
            
            }

            var newBot = new Bot
            {
                Name = request.Name,
                Context = request.Context
            };

            _db.Bots.Add(newBot);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBots), new { id = newBot.Id }, newBot);
        }

        [HttpGet("{id}/messages")]
        public async Task<IActionResult> GetMessages(int id)
        {
            var messages = await _db.Messages
                .Where(m => m.BotId == id)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            return Ok(messages);
        }

        public class BotRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Context { get; set; } = string.Empty;
        }
    }
}
