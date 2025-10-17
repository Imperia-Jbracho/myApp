using System;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Models.Travel.Dto
{
    public class CreateLodgingMilestoneRequest
    {
        [Required]
        [MaxLength(200)]
        public string? Title { get; set; }

        [Range(0, 365)]
        public int? Nights { get; set; }

        public DateOnly? CheckInDate { get; set; }

        public DateOnly? CheckOutDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Cost { get; set; }

        [MaxLength(500)]
        [Url]
        public string? LocationUrl { get; set; }

        [MaxLength(150)]
        public string? BookingPlatform { get; set; }
    }
}
