using Microsoft.AspNetCore.Mvc;
using RomanNumberConverterApp.Ui.Models;
using System.Diagnostics;

namespace RomanNumberConverterApp.Ui.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRomanNumberConverter _converter;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IRomanNumberConverter converter, ILogger<HomeController> logger)
        {
            _converter = converter;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ConvertViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(ConvertViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.RomanNumber = _converter.Convert(model.Number);
                }
                catch (ArgumentException e)
                {
                    model.RomanNumber = string.Empty;
                    model.ErrorMessage = e.Message;
                }
                ModelState.Clear();
            }
            else
            {
                model.ErrorMessage = ModelState.First().Value?.Errors.First().ErrorMessage ?? "Unexpected error";
            }
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}