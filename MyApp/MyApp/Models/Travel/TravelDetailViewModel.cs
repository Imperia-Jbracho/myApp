using System;
using System.Collections.Generic;
using System.Linq;

namespace MyApp.Models.Travel
{
    public class TravelDetailViewModel
    {
        public TravelViewModel Travel { get; set; } = new TravelViewModel();

        public DateTime ReferenceDate { get; set; } = DateTime.UtcNow;

        public int GetTotalDays()
        {
            int totalDays = (Travel.EndDate.Date - Travel.StartDate.Date).Days + 1;
            return totalDays;
        }

        public int GetTotalActivities()
        {
            if (Travel.Milestones == null)
            {
                return 0;
            }

            int totalActivities = Travel.Milestones.Count;
            return totalActivities;
        }

        public decimal GetTotalCost()
        {
            decimal totalCost = Travel.GetTotalCost();
            return totalCost;
        }

        public List<TravelMilestoneViewModel> GetUpcomingMilestones()
        {
            if (Travel.Milestones == null)
            {
                return new List<TravelMilestoneViewModel>();
            }

            List<TravelMilestoneViewModel> upcoming = Travel.Milestones
                .Where(milestone => milestone.GetStartDateTime() >= ReferenceDate)
                .OrderBy(milestone => milestone.GetStartDateTime())
                .Take(3)
                .ToList();
            return upcoming;
        }
    }
}
