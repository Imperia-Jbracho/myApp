using System;

namespace MyApp.Models.Travel
{
    public class TravelMilestoneViewModel
    {
        public Guid Id { get; set; }

        public Guid TravelId { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public decimal Cost { get; set; }

        public string Location { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public DateTime GetStartDateTime()
        {
            DateTime startDateTime = Date.Date.Add(StartTime);
            return startDateTime;
        }

        public DateTime GetEndDateTime()
        {
            DateTime endDateTime = Date.Date.Add(EndTime);
            return endDateTime;
        }
    }
}
