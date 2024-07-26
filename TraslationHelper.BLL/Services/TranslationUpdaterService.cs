using System.Text.RegularExpressions;
using TraslationHelper.Domain.Abstract.Services;

namespace TraslationHelper.BLL.Services
{
    public class TranslationUpdaterService: ITranslationUpdaterService
    {
        public string ReplaceValuesInHtmlContent(string htmlContent, Dictionary<string, string> replacements)
        {
            foreach (var kvp in replacements)
            {
                string pattern = Regex.Escape(kvp.Key); 
                string replacement = kvp.Value;
                htmlContent = Regex.Replace(htmlContent, pattern, replacement, RegexOptions.IgnoreCase);
            }

            return htmlContent;
        }
    }
}
