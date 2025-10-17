using System;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Models.Travel.Dto
{
    public class CreateRestaurantMilestoneRequest
    {
        [Required]
        [MaxLength(200)]
        public string? Title { get; set; }

        [Required]
        public DateOnly? ReservationDate { get; set; }

        public TimeOnly? StartTime { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Cost { get; set; }

        [MaxLength(500)]
        [Url]
        public string? LocationUrl { get; set; }
    }
}
