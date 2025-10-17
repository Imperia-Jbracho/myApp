using System;
using System.Collections.Generic;

namespace MyApp.Models.Travel.ViewModels
{
    public class TravelSummaryViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int DurationDays { get; set; }

        public int ActivitiesCount { get; set; }

        public decimal TotalCost { get; set; }

        public decimal InitialBudget { get; set; }

        public string Currency { get; set; } = string.Empty;

        public int ProgressPercentage { get; set; }

        public string CurrentStatus { get; set; } = string.Empty;

        public string LodgingName { get; set; } = "Sin información";

        public decimal LodgingNightlyRate { get; set; }

        public List<UpcomingEventViewModel> UpcomingEvents { get; set; } = new List<UpcomingEventViewModel>();

        public decimal BudgetDifference => TotalCost - InitialBudget;
    }
}
