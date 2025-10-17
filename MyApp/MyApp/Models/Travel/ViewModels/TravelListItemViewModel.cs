using System;
using System.Collections.Generic;

namespace MyApp.Models.Travel.ViewModels
{
    public class TravelListItemViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string Currency { get; set; } = string.Empty;

        public decimal InitialBudget { get; set; }

        public decimal TotalCost { get; set; }

        public int DurationDays { get; set; }

        public int? Ranking { get; set; }

        public bool IsArchived { get; set; }

        public bool IsFuture { get; set; }

        public bool IsOngoing { get; set; }

        public bool IsPast { get; set; }

        public int ProgressPercentage { get; set; }

        public int ActivitiesCount { get; set; }

        public decimal BudgetDifference => TotalCost - InitialBudget;

        public List<TravelParticipantViewModel> Participants { get; set; } = new List<TravelParticipantViewModel>();
    }
}
