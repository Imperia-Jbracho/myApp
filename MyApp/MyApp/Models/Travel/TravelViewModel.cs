using System;
using System.Collections.Generic;
using System.Linq;

namespace MyApp.Models.Travel
{
    public class TravelViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Destination { get; set; } = string.Empty;

        public decimal InitialBudget { get; set; }

        public int? Ranking { get; set; }

        public bool IsArchived { get; set; }

        public decimal LodgingCostPerNight { get; set; }

        public List<TravelParticipantViewModel> Participants { get; set; } = new List<TravelParticipantViewModel>();

        public List<TravelMilestoneViewModel> Milestones { get; set; } = new List<TravelMilestoneViewModel>();

        public decimal GetTotalCost()
        {
            if (Milestones == null)
            {
                return 0m;
            }

            decimal totalCost = Milestones.Sum(milestone => milestone.Cost);
            return totalCost;
        }

        public bool IsUpcoming(DateTime referenceDate)
        {
            bool isUpcoming = StartDate.Date > referenceDate.Date && !IsArchived;
            return isUpcoming;
        }

        public bool IsPast(DateTime referenceDate)
        {
            bool isPast = EndDate.Date < referenceDate.Date && !IsArchived;
            return isPast;
        }

        public int GetProgressPercentage(DateTime referenceDate)
        {
            if (referenceDate.Date <= StartDate.Date)
            {
                return 0;
            }

            if (referenceDate.Date >= EndDate.Date)
            {
                return 100;
            }

            int totalDays = (EndDate.Date - StartDate.Date).Days;
            if (totalDays == 0)
            {
                return 100;
            }

            int elapsedDays = (referenceDate.Date - StartDate.Date).Days;
            double progress = (double)elapsedDays / totalDays;
            int percentage = (int)Math.Round(progress * 100, MidpointRounding.AwayFromZero);
            return percentage;
        }
    }
}
