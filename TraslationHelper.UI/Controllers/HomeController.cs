using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TraslationHelper.Domain.Abstract.Services;
using TraslationHelper.UI.Models;

namespace TraslationHelper.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGoogleDocTranslationService _googleDocTranslationService;

        public HomeController(ILogger<HomeController> logger, IGoogleDocTranslationService googleDocTranslationService)
        {
            _logger = logger;
            _googleDocTranslationService = googleDocTranslationService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _googleDocTranslationService.ExtractTranslationsAsync("1SVmwfOWfx1OnTDA6pzI5yUuNWkUg0tmSynb5o5s1XdI");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
