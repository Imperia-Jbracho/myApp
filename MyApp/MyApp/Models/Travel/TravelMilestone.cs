using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models.Travel
{
    public class TravelMilestone
    {
        public int Id { get; set; }

        public int TravelId { get; set; }

        public Travel? Travel { get; set; }

        [Required]
        public TravelMilestoneType Type { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        [Range(0, 168)]
        public double? DurationHours { get; set; }

        [MaxLength(200)]
        public string? Classification { get; set; }

        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? LocationUrl { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal Cost { get; set; }

        [Range(0, 365)]
        public int? Nights { get; set; }

        public DateOnly? CheckInDate { get; set; }

        public DateOnly? CheckOutDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal? NightlyRate { get; set; }

        [MaxLength(150)]
        public string? BookingPlatform { get; set; }

        public DateOnly? ReservationDate { get; set; }
    }
}
