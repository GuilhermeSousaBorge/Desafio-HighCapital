namespace backend.Models
{
    public class Message
    {

        public int Id { get; set; }

        public string Sender { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int BotId { get; set; }
        public Bot Bot { get; set; } = null!;

    }
}
