using System;

namespace MyApp.Models.Travel.ViewModels
{
    public class RestaurantMilestoneViewModel
    {
        public string Title { get; set; } = string.Empty;

        public DateOnly? ReservationDate { get; set; }

        public TimeOnly? StartTime { get; set; }

        public decimal Cost { get; set; }

        public string? LocationUrl { get; set; }
    }
}
