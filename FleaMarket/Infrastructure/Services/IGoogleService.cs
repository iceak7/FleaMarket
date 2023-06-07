namespace FleaMarket.Infrastructure.Services
{
    public interface IGoogleService
    {
        Task<string> TranslateToEn(string text);
    }
}
