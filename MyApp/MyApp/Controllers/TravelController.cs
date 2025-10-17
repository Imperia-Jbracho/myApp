using System;
using Microsoft.AspNetCore.Mvc;
using MyApp.Infrastructure.Data;
using MyApp.Models.Travel;

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

        public IActionResult Details(Guid id)
        {
            TravelViewModel? travel = TravelDataStore.GetTravel(id);
            if (travel == null)
            {
                return NotFound();
            }

            TravelDetailViewModel detailViewModel = new TravelDetailViewModel
            {
                Travel = travel,
                ReferenceDate = DateTime.UtcNow
            };

            return View(detailViewModel);
        }
    }
}
