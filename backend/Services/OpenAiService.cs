using System.Text.Json;

namespace backend.Services
{
    // Interface para poder trocar facilmente entre Mock e Real
    public interface IOpenAiService
    {
        Task<string> GetBotResponse(string context, string userMessage);
    }

    // Serviço real da OpenAI
    public class OpenAiService : IOpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<OpenAiService> _logger;

        public OpenAiService(IConfiguration configuration, ILogger<OpenAiService> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;

            _apiKey = configuration["OpenAI:ApiKey"] ?? "";
            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
            }
        }

        public async Task<string> GetBotResponse(string context, string userMessage)
        {
            // Implementação real (a que você já tem)
            // ... seu código atual aqui
            throw new NotImplementedException("Use o MockOpenAiService para desenvolvimento");
        }
    }

    // Serviço Mock para desenvolvimento
    public class MockOpenAiService : IOpenAiService
    {
        private readonly ILogger<MockOpenAiService> _logger;

        private static readonly Dictionary<string, string> _responses = new()
        {
            { "default", "Olá! Sou um assistente virtual. Como posso ajudá-lo hoje?" },
            { "ola", "Olá! Como está indo seu dia?" },
            { "como esta", "Estou funcionando perfeitamente! E você, como está?" },
            { "ajuda", "Claro! Estou aqui para ajudar. O que você gostaria de saber?" },
            { "obrigado", "De nada! Fico feliz em poder ajudar!" },
            { "tchau", "Até logo! Foi um prazer conversar com você!" },
            { "teste", "Sistema funcionando perfeitamente! ✅" },
            { "programacao", "Adoro falar sobre programação! Qual linguagem você está usando?" },
            { "javascript", "JavaScript é uma linguagem incrível! Muito versátil." },
            { "python", "Python é excelente para iniciantes e muito poderosa!" },
            { "csharp", "C# é uma linguagem robusta, perfeita para desenvolvimento web!" }
        };

        public MockOpenAiService(ILogger<MockOpenAiService> logger)
        {
            _logger = logger;
        }

        public async Task<string> GetBotResponse(string context, string userMessage)
        {
            _logger.LogInformation($"[MOCK] Processando: {userMessage}");

            // Simula delay da API real
            await Task.Delay(Random.Shared.Next(800, 1500));

            var response = GenerateResponse(userMessage, context);

            _logger.LogInformation($"[MOCK] Resposta: {response}");
            return response;
        }

        private string GenerateResponse(string userMessage, string context)
        {
            var message = userMessage.ToLower().Trim();

            // Respostas específicas baseadas em palavras-chave
            foreach (var kvp in _responses)
            {
                if (message.Contains(kvp.Key))
                {
                    return kvp.Value;
                }
            }

            // Respostas baseadas no contexto
            if (!string.IsNullOrEmpty(context))
            {
                if (context.ToLower().Contains("matemática"))
                    return $"Como professor de matemática, posso dizer que '{userMessage}' é uma excelente pergunta!";

                if (context.ToLower().Contains("vendas"))
                    return $"Como especialista em vendas, acredito que posso ajudar com '{userMessage}'. Vamos conversar!";
            }

            // Respostas genéricas inteligentes
            var genericResponses = new[]
            {
                $"Interessante pergunta sobre '{userMessage}'. Deixe-me explicar...",
                $"Entendo que você quer saber sobre '{userMessage}'. Aqui está minha perspectiva:",
                $"Ótima questão! Sobre '{userMessage}', posso dizer que...",
                $"Baseado na sua pergunta '{userMessage}', acredito que a melhor resposta seria:",
                $"Você tocou em um ponto importante com '{userMessage}'. Vou explicar:"
            };

            return genericResponses[Random.Shared.Next(genericResponses.Length)];
        }
    }
}