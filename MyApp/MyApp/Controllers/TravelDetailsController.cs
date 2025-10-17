using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Infrastructure.Data;
using MyApp.Models.Travel;
using MyApp.Models.Travel.ViewModels;

namespace MyApp.Controllers
{
    public class TravelDetailsController : Controller
    {
        private readonly ApplicationDbContext context;

        public TravelDetailsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Summary(int id)
        {
            Travel? travel = await context.Travels
                .Include(entity => entity.Milestones)
                .FirstOrDefaultAsync(entity => entity.Id == id);

            if (travel == null)
            {
                return NotFound();
            }

            TravelSummaryViewModel viewModel = BuildSummaryViewModel(travel);

            ViewData["Title"] = string.Concat("Resumen · ", travel.Title);

            return View(viewModel);
        }

        private static TravelSummaryViewModel BuildSummaryViewModel(Travel travel)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

            bool isFuture = travel.StartDate > today;
            bool isPast = travel.EndDate < today;

            string status = isFuture ? "Próximo" : isPast ? "Finalizado" : "En curso";
            if (travel.IsArchived)
            {
                status = "Archivado";
            }

            int progress = TravelProgressCalculator.CalculateProgressPercentage(travel, today);

            decimal totalCost = travel.Milestones.Sum(milestone => milestone.Cost);

            TravelSummaryViewModel viewModel = new TravelSummaryViewModel
            {
                Id = travel.Id,
                Title = travel.Title,
                Destination = travel.Destination,
                StartDate = travel.StartDate,
                EndDate = travel.EndDate,
                DurationDays = travel.DurationDays,
                ActivitiesCount = travel.Milestones.Count,
                TotalCost = totalCost,
                InitialBudget = travel.InitialBudget,
                Currency = travel.Currency,
                ProgressPercentage = progress,
                CurrentStatus = status,
                LodgingName = travel.Destination,
                LodgingNightlyRate = 0m
            };

            viewModel.UpcomingEvents = travel.Milestones
                .Where(milestone => ToDateTime(milestone.Date, milestone.StartTime) >= DateTime.UtcNow)
                .OrderBy(milestone => milestone.Date)
                .ThenBy(milestone => milestone.StartTime)
                .Take(5)
                .Select(milestone => new UpcomingEventViewModel
                {
                    Title = string.IsNullOrWhiteSpace(milestone.Title) ? "Evento" : milestone.Title,
                    Date = milestone.Date,
                    StartTime = milestone.StartTime,
                    EndTime = milestone.EndTime,
                    Cost = milestone.Cost
                })
                .ToList();

            return viewModel;
        }

        private static DateTime ToDateTime(DateOnly date, TimeOnly time)
        {
            DateTime dateTime = date.ToDateTime(time);

            return dateTime;
        }
    }
}
