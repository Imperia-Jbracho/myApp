using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyApp.Infrastructure.Data;
using MyApp.Models;
using MyApp.Models.Travel;

namespace MyApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string filter = "futuros", string search = "")
        {
            DateTime referenceDate = DateTime.UtcNow.Date;
            List<TravelViewModel> travels = TravelDataStore.GetTravels();

            if (!string.IsNullOrWhiteSpace(search))
            {
                travels = travels
                    .Where(travel => travel.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                if (filter.Equals("futuros", StringComparison.OrdinalIgnoreCase))
                {
                    travels = travels
                        .Where(travel => travel.IsUpcoming(referenceDate))
                        .ToList();
                }
                else if (filter.Equals("pasados", StringComparison.OrdinalIgnoreCase))
                {
                    travels = travels
                        .Where(travel => travel.IsPast(referenceDate))
                        .ToList();
                }
                else if (filter.Equals("archivados", StringComparison.OrdinalIgnoreCase))
                {
                    travels = travels
                        .Where(travel => travel.IsArchived)
                        .ToList();
                }
            }

            TravelDashboardViewModel viewModel = new TravelDashboardViewModel
            {
                Travels = travels,
                SelectedFilter = filter,
                SearchTerm = search
            };

            IOrderedEnumerable<TravelViewModel> orderedTravels = travels
                .OrderBy(travel => travel.StartDate);
            TravelViewModel? highlightedTravel = orderedTravels.FirstOrDefault();

            if (highlightedTravel != null)
            {
                ViewBag.ActiveTripId = highlightedTravel.Id.ToString();
                ViewBag.ActiveTripDate = highlightedTravel.StartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                int notifications = highlightedTravel.Milestones
                    .Count(milestone => milestone.GetStartDateTime() >= referenceDate);
                ViewBag.NotificationsCount = notifications;
            }
            else
            {
                ViewBag.ActiveTripId = null;
                ViewBag.ActiveTripDate = null;
                ViewBag.NotificationsCount = 0;
            }

            return View(viewModel);
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
