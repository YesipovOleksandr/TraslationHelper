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
        public async Task<IActionResult> Upload(IFormFile htmlFile, string googleDocUrl)
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

            var updatedStream = await _streamService.GetStreamFromStringAsync(updatedHtmlContent);

            return File(updatedStream, "text/html", $"{htmlFile.Name}_modified.html");
        }

        private static string ExtractGoogleDocId(string url)
        {
            var match = Regex.Match(url, @"^https:\/\/docs\.google\.com\/document\/d\/([a-zA-Z0-9-_]+)\/");
            return match.Success ? match.Groups[1].Value : null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
