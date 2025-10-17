using System;
using System.Globalization;
using System.Linq;
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

            ViewBag.ActiveTripId = travel.Id.ToString();
            ViewBag.ActiveTripDate = travel.StartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            int pendingMilestones = travel.Milestones
                .Count(milestone => milestone.GetStartDateTime() >= DateTime.UtcNow);
            ViewBag.NotificationsCount = pendingMilestones;

            TravelDetailViewModel detailViewModel = new TravelDetailViewModel
            {
                Travel = travel,
                ReferenceDate = DateTime.UtcNow
            };

            return View(detailViewModel);
        }
    }
}
