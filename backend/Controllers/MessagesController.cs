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
        //private readonly OpenAiService _openAiService;
        private readonly IOpenAiService _openAiService;

        public MessagesController(ChatDbContext db, IOpenAiService openAiService)
        {
            _db = db;
            _openAiService = openAiService;
        }

        [HttpPost("Bot/{id}/chat")]
        public async Task<IActionResult> SendMessage(int id, [FromBody] ChatRequest request)
        {

            var bot = await _db.Bots.FindAsync(id);
            if (bot == null)
            {
                return NotFound("Bot not found.");
            }
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Message cannot be empty.");
            }

            var response = await _openAiService.GetBotResponse(bot.Context, request.Content);

            var message = new Models.Message
            {
                Sender = "User",
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                BotId = id
            };

            _db.Messages.Add(message);
            await _db.SaveChangesAsync();

            var botMessage = new Models.Message
            {
                Sender = "Bot",
                Content = response,
                CreatedAt = DateTime.UtcNow,
                BotId = id
            };

            _db.Messages.Add(botMessage);
            await _db.SaveChangesAsync();

            return Ok(new 
            {
                userMessage = new
                {
                    id = message.Id,
                    content = message.Content,
                    sender = message.Sender,
                    createdAt = message.CreatedAt
                },
                botMessage = new
                {
                    id = botMessage.Id,
                    content = botMessage.Content,
                    sender = botMessage.Sender,
                    createdAt = botMessage.CreatedAt
                }


            });
        }

        public class ChatRequest
        {
            public string Content { get; set; } = string.Empty;
        }

    }
}
