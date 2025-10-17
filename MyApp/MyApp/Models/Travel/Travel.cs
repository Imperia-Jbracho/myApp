using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models.Travel
{
    public class Travel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Required]
        [MaxLength(150)]
        public string Destination { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Currency { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int DurationDays { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal InitialBudget { get; set; }

        [Range(0, 5)]
        public int? Ranking { get; set; }

        public bool IsArchived { get; set; }

        public ICollection<TravelParticipant> Participants { get; set; } = new List<TravelParticipant>();

        public ICollection<TravelMilestone> Milestones { get; set; } = new List<TravelMilestone>();
    }
}
