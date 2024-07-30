using HtmlAgilityPack;
using System.Text.RegularExpressions;
using TraslationHelper.Domain.Abstract.Services;

namespace TraslationHelper.BLL.Services
{
    public class TranslationUpdaterService: ITranslationUpdaterService
    {
        const int numberOfLetters = 5;
        public string ReplaceValuesExactMatchInHtmlContent(string htmlContent, Dictionary<string, string> replacements)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//text()");

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var text = node.InnerText;

                    string trimmedText = Regex.Replace(text, @"^\s*[\.,;:!?\-–]*", "").Trim();

                    if (string.IsNullOrEmpty(trimmedText))
                    {
                        continue;
                    }

                    if (replacements.ContainsKey(trimmedText.ToLower()))
                    {
                        node.InnerHtml = replacements[trimmedText.ToLower()];
                    }
                }
            }

            return htmlDoc.DocumentNode.OuterHtml;
        }

        public string ReplaceValuesInHtmlContent(string htmlContent, Dictionary<string, string> replacements)
        {
            foreach (var kvp in replacements)
            {
                if (kvp.Key.Length > numberOfLetters)
                {
                    string pattern = Regex.Escape(kvp.Key);
                    string replacement = kvp.Value;
                    htmlContent = Regex.Replace(htmlContent, pattern, replacement, RegexOptions.IgnoreCase);
                }
            }
            return htmlContent;
        }
    }
}
