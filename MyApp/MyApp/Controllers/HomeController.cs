using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Infrastructure.Data;
using MyApp.Models;
using MyApp.Models.Travel;
using MyApp.Models.Travel.ViewModels;

namespace MyApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly ApplicationDbContext context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? filter, string? search)
        {
            TravelListViewModel viewModel = await BuildListViewModelAsync(filter, search);

            ViewData["Title"] = "Mis viajes";

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ErrorViewModel model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(model);
        }

        private async Task<TravelListViewModel> BuildListViewModelAsync(string? filter, string? search)
        {
            string normalizedFilter = string.IsNullOrWhiteSpace(filter) ? "todos" : filter.Trim().ToLowerInvariant();
            string normalizedSearch = string.IsNullOrWhiteSpace(search) ? string.Empty : search.Trim();

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

            List<Travel> travels = await context.Travels
                .Include(travel => travel.Participants)
                .Include(travel => travel.Milestones)
                .OrderBy(travel => travel.StartDate)
                .ToListAsync();

            int totalCount = travels.Count;
            int futureCount = travels
                .Count(travel => !travel.IsArchived && travel.StartDate > today);
            int pastCount = travels
                .Count(travel => !travel.IsArchived && travel.EndDate < today);
            int archivedCount = travels
                .Count(travel => travel.IsArchived);

            IEnumerable<Travel> filteredTravels = travels;

            switch (normalizedFilter)
            {
                case "futuros":
                    filteredTravels = travels
                        .Where(travel => !travel.IsArchived && travel.StartDate > today);
                    break;
                case "pasados":
                    filteredTravels = travels
                        .Where(travel => !travel.IsArchived && travel.EndDate < today);
                    break;
                case "archivados":
                    filteredTravels = travels
                        .Where(travel => travel.IsArchived);
                    break;
                default:
                    normalizedFilter = "todos";
                    filteredTravels = travels
                        .Where(travel => !travel.IsArchived);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(normalizedSearch))
            {
                string searchLower = normalizedSearch.ToLowerInvariant();

                filteredTravels = filteredTravels
                    .Where(travel => travel.Title.ToLowerInvariant().Contains(searchLower));
            }

            List<TravelListItemViewModel> items = filteredTravels
                .Select(travel => MapToListItem(travel, today))
                .ToList();

            TravelListViewModel viewModel = new TravelListViewModel
            {
                Travels = items,
                ActiveFilter = normalizedFilter,
                SearchTerm = normalizedSearch,
                TotalCount = totalCount,
                FutureCount = futureCount,
                PastCount = pastCount,
                ArchivedCount = archivedCount,
                Form = BuildEmptyFormModel()
            };

            return viewModel;
        }

        private static TravelListItemViewModel MapToListItem(Travel travel, DateOnly today)
        {
            bool isFuture = travel.StartDate > today;
            bool isPast = travel.EndDate < today;
            bool isOngoing = !isFuture && !isPast;

            int progress = TravelProgressCalculator.CalculateProgressPercentage(travel, today);

            decimal totalCost = travel.Milestones
                .Sum(milestone => milestone.Cost);

            int activitiesCount = travel.Milestones
                .Count(milestone => milestone.Type == TravelMilestoneType.Activity);

            List<TravelParticipantViewModel> participants = travel.Participants
                .OrderBy(participant => participant.Email)
                .Select(participant => new TravelParticipantViewModel
                {
                    Email = participant.Email,
                    Role = participant.Role
                })
                .ToList();

            TravelListItemViewModel item = new TravelListItemViewModel
            {
                Id = travel.Id,
                Title = travel.Title,
                Destination = travel.Destination,
                StartDate = travel.StartDate,
                EndDate = travel.EndDate,
                Currency = travel.Currency,
                InitialBudget = travel.InitialBudget,
                TotalCost = totalCost,
                DurationDays = travel.DurationDays,
                Ranking = travel.Ranking,
                IsArchived = travel.IsArchived,
                IsFuture = isFuture,
                IsPast = isPast,
                IsOngoing = isOngoing,
                ProgressPercentage = progress,
                ActivitiesCount = activitiesCount,
                Participants = participants
            };

            return item;
        }

        private static TravelFormViewModel BuildEmptyFormModel()
        {
            TravelFormViewModel form = new TravelFormViewModel
            {
                Currency = "USD",
                InitialBudget = 0m
            };

            form.Participants.Add(new TravelParticipantFormViewModel());

            return form;
        }
    }
}
