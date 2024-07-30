namespace TraslationHelper.Domain.Abstract.Services
{
    public interface ITranslationUpdaterService
    {
        public string ReplaceValuesInHtmlContent(string htmlContent, Dictionary<string, string> replacements);

        public string ReplaceValuesExactMatchInHtmlContent(string htmlContent, Dictionary<string, string> replacements);
    }
}
