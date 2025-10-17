using System;

namespace MyApp.Models.Travel.ViewModels
{
    public class ActivityMilestoneViewModel
    {
        public string Title { get; set; } = string.Empty;

        public DateOnly Date { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public double? DurationHours { get; set; }

        public string? Classification { get; set; }

        public decimal Cost { get; set; }

        public string? LocationUrl { get; set; }
    }
}
