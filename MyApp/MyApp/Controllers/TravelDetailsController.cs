using System;
using System.Collections.Generic;
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

            List<TravelMilestone> milestones = travel.Milestones
                .OrderBy(milestone => milestone.Date)
                .ThenBy(milestone => milestone.StartTime)
                .ToList();

            decimal totalCost = milestones.Sum(milestone => milestone.Cost);

            List<TravelMilestone> activityMilestones = milestones
                .Where(milestone => milestone.Type == TravelMilestoneType.Activity)
                .ToList();

            List<TravelMilestone> lodgingMilestones = milestones
                .Where(milestone => milestone.Type == TravelMilestoneType.Lodging)
                .ToList();

            List<TravelMilestone> restaurantMilestones = milestones
                .Where(milestone => milestone.Type == TravelMilestoneType.Restaurant)
                .ToList();

            int elapsedDays = CalculateElapsedDays(travel.StartDate, travel.EndDate, today);
            int remainingDays = Math.Max(0, travel.DurationDays - elapsedDays);

            TravelSummaryViewModel viewModel = new TravelSummaryViewModel
            {
                Id = travel.Id,
                Title = travel.Title,
                Destination = travel.Destination,
                StartDate = travel.StartDate,
                EndDate = travel.EndDate,
                DurationDays = travel.DurationDays,
                DaysElapsed = elapsedDays,
                DaysRemaining = remainingDays,
                ActivitiesCount = activityMilestones.Count,
                TotalCost = totalCost,
                InitialBudget = travel.InitialBudget,
                Currency = travel.Currency,
                ProgressPercentage = progress,
                CurrentStatus = status,
                LodgingName = travel.Destination,
                LodgingNightlyRate = 0m
            };

            viewModel.Activities = activityMilestones
                .Select(milestone => new ActivityMilestoneViewModel
                {
                    Title = string.IsNullOrWhiteSpace(milestone.Title) ? "Actividad" : milestone.Title,
                    Date = milestone.Date,
                    StartTime = milestone.StartTime,
                    EndTime = milestone.EndTime,
                    DurationHours = milestone.DurationHours,
                    Classification = milestone.Classification,
                    Cost = milestone.Cost,
                    LocationUrl = milestone.LocationUrl
                })
                .ToList();

            viewModel.Lodgings = lodgingMilestones
                .Select(milestone => new LodgingMilestoneViewModel
                {
                    Title = string.IsNullOrWhiteSpace(milestone.Title) ? "Alojamiento" : milestone.Title,
                    Nights = milestone.Nights,
                    CheckInDate = milestone.CheckInDate,
                    CheckOutDate = milestone.CheckOutDate,
                    Cost = milestone.Cost,
                    NightlyRate = CalculateNightlyRate(milestone),
                    LocationUrl = milestone.LocationUrl,
                    BookingPlatform = milestone.BookingPlatform
                })
                .ToList();

            viewModel.Restaurants = restaurantMilestones
                .Select(milestone => new RestaurantMilestoneViewModel
                {
                    Title = string.IsNullOrWhiteSpace(milestone.Title) ? "Restaurante" : milestone.Title,
                    ReservationDate = milestone.ReservationDate,
                    StartTime = milestone.StartTime,
                    Cost = milestone.Cost,
                    LocationUrl = milestone.LocationUrl
                })
                .ToList();

            UpdateLodgingDetails(viewModel);

            List<UpcomingEventViewModel> upcomingEvents = new List<UpcomingEventViewModel>();

            IEnumerable<TravelMilestone> scheduledMilestones = milestones
                .Where(milestone => milestone.StartTime.HasValue)
                .OrderBy(milestone => milestone.Date)
                .ThenBy(milestone => milestone.StartTime);

            foreach (TravelMilestone milestone in scheduledMilestones)
            {
                TimeOnly startTimeValue = milestone.StartTime.GetValueOrDefault();
                DateTime scheduledDateTime = ToDateTime(milestone.Date, startTimeValue);

                if (scheduledDateTime < DateTime.UtcNow)
                {
                    continue;
                }

                TimeOnly endTimeValue = milestone.EndTime ?? startTimeValue;

                UpcomingEventViewModel eventViewModel = new UpcomingEventViewModel
                {
                    Title = string.IsNullOrWhiteSpace(milestone.Title) ? "Evento" : milestone.Title,
                    Date = milestone.Date,
                    StartTime = startTimeValue,
                    EndTime = endTimeValue,
                    Cost = milestone.Cost,
                    Type = milestone.Type,
                    LocationUrl = milestone.LocationUrl
                };

                upcomingEvents.Add(eventViewModel);

                if (upcomingEvents.Count >= 5)
                {
                    break;
                }
            }

            viewModel.UpcomingEvents = upcomingEvents;

            return viewModel;
        }

        private static DateTime ToDateTime(DateOnly date, TimeOnly time)
        {
            DateTime dateTime = date.ToDateTime(time);

            return dateTime;
        }

        private static int CalculateElapsedDays(DateOnly startDate, DateOnly endDate, DateOnly referenceDate)
        {
            if (referenceDate < startDate)
            {
                return 0;
            }

            int effectiveEnd = Math.Min(referenceDate.DayNumber, endDate.DayNumber);
            int elapsed = effectiveEnd - startDate.DayNumber + 1;

            if (elapsed < 0)
            {
                return 0;
            }

            return elapsed;
        }

        private static decimal? CalculateNightlyRate(TravelMilestone milestone)
        {
            if (milestone.NightlyRate.HasValue)
            {
                return milestone.NightlyRate.Value;
            }

            if (milestone.Nights.HasValue && milestone.Nights.Value > 0)
            {
                decimal nightsValue = milestone.Nights.Value;
                decimal nightlyRate = milestone.Cost / nightsValue;

                return decimal.Round(nightlyRate, 2, MidpointRounding.AwayFromZero);
            }

            return null;
        }

        private static void UpdateLodgingDetails(TravelSummaryViewModel viewModel)
        {
            LodgingMilestoneViewModel? currentLodging = viewModel.Lodgings
                .OrderByDescending(lodging => lodging.CheckInDate ?? DateOnly.MinValue)
                .FirstOrDefault();

            if (currentLodging == null)
            {
                return;
            }

            viewModel.LodgingName = currentLodging.Title;
            viewModel.LodgingNightlyRate = currentLodging.NightlyRate ?? 0m;
            viewModel.LodgingCheckInDate = currentLodging.CheckInDate;
            viewModel.LodgingCheckOutDate = currentLodging.CheckOutDate;
            viewModel.LodgingLocationUrl = currentLodging.LocationUrl;
            viewModel.LodgingBookingPlatform = currentLodging.BookingPlatform;
        }
    }
}
