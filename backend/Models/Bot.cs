namespace backend.Models
{
    public class Bot
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Context { get; set; } = null!;
        public List<Message> Messages { get; set; } = [];
    }
}
