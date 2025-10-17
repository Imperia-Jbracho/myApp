using System;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Models.Travel.Dto
{
    public class CreateActivityMilestoneRequest
    {
        [Required]
        [MaxLength(200)]
        public string? Title { get; set; }

        [Required]
        public DateOnly? Date { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        [Range(0, 168)]
        public double? DurationHours { get; set; }

        [MaxLength(200)]
        public string? Classification { get; set; }

        [MaxLength(500)]
        [Url]
        public string? LocationUrl { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Cost { get; set; }
    }
}
