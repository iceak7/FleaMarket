using OpenAI_API;
using OpenAI_API.Completions;

namespace FleaMarket.Infrastructure.Services
{
    public class OpenApiService : IOpenApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public OpenApiService(IHttpClientFactory httpClientFactory)
        {

            _httpClientFactory = httpClientFactory;

        }

        public Task<string> GenereateTitle(string description)
        {
            throw new NotImplementedException();
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

                CompletionRequest completionRequest = new CompletionRequest()
                {
                    Model = "text-curie-001",
                    Prompt = "Översätt följande text till engelska: \"" + text + "\"" ,
                    Temperature = 0.3,
                    MaxTokens = 500
                };

                var res = await api.Completions.CreateCompletionAsync(completionRequest);

                return res.Completions[0].Text;
            }
            catch(Exception ex)
            {
                return null;
            }



        }
    }
}
