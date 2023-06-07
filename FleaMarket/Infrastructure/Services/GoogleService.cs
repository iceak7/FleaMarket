using Google.Cloud.Translation.V2;
using Google.Apis.Auth.OAuth2;

namespace FleaMarket.Infrastructure.Services
{
    public class GoogleService : IGoogleService
    {
        private readonly TranslationClient _client;
        public GoogleService()
        {
            _client = TranslationClient.Create();
        }
        public async Task<string> TranslateToEn(string text)
        {

            var translatedText = await _client.TranslateTextAsync(text, LanguageCodes.English, LanguageCodes.Swedish);

            return translatedText.TranslatedText;
        }
    }
}
