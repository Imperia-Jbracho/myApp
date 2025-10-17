using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    public class TravelController : Controller
    {
        public IActionResult Destinos()
        {
            return View();
        }

        public IActionResult Itinerario()
        {
            return View();
        }

        public IActionResult LocalizadorDeVuelos()
        {
            return View();
        }
    }
}
