namespace backend.Interfaces
{
    public interface IOpenAiService
    {
        Task<string> GetBotResponse(string context, string userMessage);
    }
}
