using System.Text;
using System.Text.Json;

namespace backend.Services
{
    public class OpenAiService
    {

        private readonly HttpClient _httpClient;
        private string _apiKey;

        public OpenAiService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["OpenAI:ApiKey"] ?? throw new ArgumentNullException("OpenAI API Key is not configured.");

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> GetBotResponse(string context, string userMessage)
        {
            var payload = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = context },
                    new { role = "user", content = userMessage }
                }
            };

            var content = new StringContent(
                 JsonSerializer.Serialize(payload),
                 Encoding.UTF8,
                 "application/json"
             );

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            if (!response.IsSuccessStatusCode) throw new Exception($"Erro da OpenAI: {response.StatusCode}");

            var responseJson = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(responseJson);

            var message = doc
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return message?.Trim() ?? "Erro ao interpretar resposta do bot.";
        }
    }
}
