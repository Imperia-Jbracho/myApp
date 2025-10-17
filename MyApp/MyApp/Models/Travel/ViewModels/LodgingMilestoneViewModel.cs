using System;

namespace MyApp.Models.Travel.ViewModels
{
    public class LodgingMilestoneViewModel
    {
        public string Title { get; set; } = string.Empty;

        public int? Nights { get; set; }

        public DateOnly? CheckInDate { get; set; }

        public DateOnly? CheckOutDate { get; set; }

        public decimal Cost { get; set; }

        public decimal? NightlyRate { get; set; }

        public string? LocationUrl { get; set; }

        public string? BookingPlatform { get; set; }
    }
}
