using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TraslationHelper.Domain.Abstract.Services;
using TraslationHelper.UI.Models;

namespace TraslationHelper.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGoogleDocTranslationService _googleDocTranslationService;
        private readonly IStreamService _streamService;
        private readonly ITranslationUpdaterService _translationUpdaterService; 

        public HomeController(ILogger<HomeController> logger,
                              IGoogleDocTranslationService googleDocTranslationService, 
                              ITranslationUpdaterService translationUpdaterService,
                              IStreamService streamService)
        {
            _logger = logger;
            _googleDocTranslationService = googleDocTranslationService;
            _translationUpdaterService = translationUpdaterService; 
            _streamService = streamService;
        }

        public async Task<IActionResult> Index()
        {          
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile htmlFile, string googleDocUrl, bool highlight)
        {
            if (string.IsNullOrWhiteSpace(googleDocUrl))
            {
                return BadRequest("Google Doc URL is required.");
            }

            string documentId = ExtractGoogleDocId(googleDocUrl);
            if (string.IsNullOrWhiteSpace(documentId))
            {
                return BadRequest("Invalid Google Doc URL.");
            }

            var replacementWords = await _googleDocTranslationService.ExtractTranslationsAsync(documentId);

            if (replacementWords == null)
            {
                return BadRequest("Invalid replacements data.");
            }

            if (htmlFile == null || htmlFile.Length == 0)
            {
                return BadRequest("File is required");
            }

            string htmlContent = await _streamService.ReadStreamContentAsync(htmlFile.OpenReadStream());

            string updatedHtmlContent = _translationUpdaterService.ReplaceValuesInHtmlContent(htmlContent, replacementWords);

            if (highlight)
            {
                updatedHtmlContent = AddHighlightScriptAndStyles(updatedHtmlContent);
            }

            var updatedStream = await _streamService.GetStreamFromStringAsync(updatedHtmlContent);

            return File(updatedStream, "text/html", $"{htmlFile.Name}_modified.html");
        }

        private static string ExtractGoogleDocId(string url)
        {
            var match = Regex.Match(url, @"^https:\/\/docs\.google\.com\/document\/d\/([a-zA-Z0-9-_]+)\/");
            return match.Success ? match.Groups[1].Value : null;
        }

        private string AddHighlightScriptAndStyles(string htmlContent)
        {
            var script = @"
                <script>
                    function highlightRussianText(element) {
                        if (element.nodeType === 3) {
                            const russianCharPattern = /[а-€ј-яЄ®]/g;
                            const parent = element.parentNode;
                            const text = element.nodeValue;
                
                            if (russianCharPattern.test(text)) {
                                const newHTML = text.replace(russianCharPattern, '<span class=""highlight"">$&</span>');
                                const tempDiv = document.createElement('div');
                                tempDiv.innerHTML = newHTML;
                                while (tempDiv.firstChild) {
                                    parent.insertBefore(tempDiv.firstChild, element);
                                }
                                parent.removeChild(element);
                            }
                        } else if (element.nodeType === 1 && element.nodeName !== 'SCRIPT' && element.nodeName !== 'STYLE') { // Ёлементный узел
                            for (let i = 0; i < element.childNodes.length; i++) {
                                highlightRussianText(element.childNodes[i]);
                            }
                        }
                    }

                    document.addEventListener('DOMContentLoaded', function () {
                        highlightRussianText(document.body);
                    });
                </script>";

                        var styles = @"
                <style>
                    .highlight {
                        background-color: red; 
                        font-weight: bold;
                    }
                </style>";

            var closingBodyIndex = htmlContent.LastIndexOf("</body>");
            if (closingBodyIndex >= 0)
            {
                htmlContent = htmlContent.Insert(closingBodyIndex, script + styles);
            }

            return htmlContent;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
