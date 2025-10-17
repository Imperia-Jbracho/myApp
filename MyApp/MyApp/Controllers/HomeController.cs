using System;
using System.Collections.Generic;
using System.Diagnostics;
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
