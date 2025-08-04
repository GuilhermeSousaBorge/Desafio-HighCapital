using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {

        private readonly ChatDbContext _db;
        private readonly OpenAiService _openAiService;

        public MessagesController(ChatDbContext db, OpenAiService openAiService)
        {
            _db = db;
            _openAiService = openAiService;
        }

        [HttpPost("/Bot/{id}/chat")]
        public async Task<IActionResult> SendMessage(int id, [FromBody] ChatRequest request)
        {

            var bot = await _db.Bots.FindAsync(id);
            if (bot == null)
            {
                return NotFound("Bot not found.");
            }
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Message cannot be empty.");
            }

            var response = await _openAiService.GetBotResponse(bot.Context, request.Message);

            var message = new Models.Message
            {
                Sender = "User",
                Content = request.Message,
                CreatedAt = DateTime.UtcNow,
                BotId = id
            };

            _db.Messages.Add(message);
            await _db.SaveChangesAsync();

            return Ok(new { response });
        }

        public class ChatRequest
        {
            public string Message { get; set; } = string.Empty;
        }

    }
}
