namespace TraslationHelper.Domain.Abstract.Services
{
    public interface IGoogleDocTranslationService
    {
        public Task<Dictionary<string, string>> ExtractTranslationsAsync(string documentId);
    }
}
