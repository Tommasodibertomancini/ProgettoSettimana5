using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgettoSettimana5.Models;
using ProgettoSettimana5.Services;
using ProgettoSettimana5.ViewModels;

namespace ProgettoSettimana5.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly PrenotazioneService _prenotazioneService;

        public HomeController(PrenotazioneService prenotazioneService)
        {
            _prenotazioneService = prenotazioneService;
        }

        public async Task<IActionResult> Index()
        {
            // Dashboard 
            ViewBag.PrenotazioniAttuali = await _prenotazioneService.GetPrenotazioniAttualiAsync();
            ViewBag.PrenotazioniFuture = await _prenotazioneService.GetPrenotazioniFutureAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
