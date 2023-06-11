using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Completions;
using OpenAI_API.Models;
using static System.Net.Mime.MediaTypeNames;

namespace FleaMarket.Infrastructure.Services
{
    public class OpenApiService : IOpenApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public OpenApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GenerateTitle(string description, IEnumerable<string> titles)
        {
            try
            {
                OpenAIAPI api = new OpenAIAPI(APIAuthentication.LoadFromEnv());
                //api.HttpClientFactory = _httpClientFactory;

                var previousTitles = string.Join(",", titles.Select(x => "\"" + x +"\""));

                var prompt = "Svara med en lämplig titel på ett föremålet baserad på följande beskrivning av föremålet: \"" + description + "\". Här är titlarna på de andra föremålen: " + previousTitles + ". Svara endast med en titel och på svenska.";

                ChatRequest chatRequest = new ChatRequest()
                {
                    Model = Model.ChatGPTTurbo,
                    Messages = new ChatMessage[]
                    {   
                        new ChatMessage(ChatMessageRole.User, prompt )
                    },
                    Temperature = 0.05,
                    MaxTokens = 500,
                };

                var res = await api.Chat.CreateChatCompletionAsync(chatRequest);

                var title = res.ToString().Trim('"');

                return title;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Task<string> RefineText(string text)
        {
            throw new NotImplementedException();
        }

        public async Task<string> TranslateToEn(string text)
        {
            try
            {
                OpenAIAPI api = new OpenAIAPI(APIAuthentication.LoadFromEnv());
                //api.HttpClientFactory = _httpClientFactory;

                ChatRequest chatRequest = new ChatRequest()
                {
                    Model = Model.ChatGPTTurbo,
                    Messages = new ChatMessage[] 
                    {   new ChatMessage(ChatMessageRole.User, "Översätt följande text till engelska: \"Det här är en beskrivning.\""),
                        new ChatMessage(ChatMessageRole.Assistant, "This is a description."),
                        new ChatMessage(ChatMessageRole.User, "Översätt följande text till engelska: \"" + text + "\"" )
                    },
                    Temperature = 0.2,
                    MaxTokens = 500     ,
                };

                var res = await api.Chat.CreateChatCompletionAsync(chatRequest);

                return res.Choices[0].Message.Content;
            }
            catch(Exception ex)
            {
                return null;
            }



        }
    }
}
