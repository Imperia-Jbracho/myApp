using System;
using MyApp.Models.Travel;

namespace MyApp.Models.Travel.Dto
{
    public class TravelMilestoneDto
    {
        public int Id { get; set; }

        public int TravelId { get; set; }

        public TravelMilestoneType Type { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateOnly Date { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public double? DurationHours { get; set; }

        public string? Classification { get; set; }

        public string? LocationUrl { get; set; }

        public decimal Cost { get; set; }

        public int? Nights { get; set; }

        public DateOnly? CheckInDate { get; set; }

        public DateOnly? CheckOutDate { get; set; }

        public decimal? NightlyRate { get; set; }

        public string? BookingPlatform { get; set; }

        public DateOnly? ReservationDate { get; set; }
    }
}
