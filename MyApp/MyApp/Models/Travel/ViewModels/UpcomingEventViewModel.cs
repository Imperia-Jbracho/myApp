using System;

namespace MyApp.Models.Travel.ViewModels
{
    public class UpcomingEventViewModel
    {
        public string Title { get; set; } = string.Empty;

        public DateOnly Date { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public decimal Cost { get; set; }
    }
}
