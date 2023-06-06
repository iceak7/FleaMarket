namespace FleaMarket.Infrastructure.Services
{
    public interface IOpenApiService
    {
        Task<string> TranslateToEn(string text);
        Task<string> RefineText(string text);
        Task<string> GenereateTitle(string description);
    }
}
