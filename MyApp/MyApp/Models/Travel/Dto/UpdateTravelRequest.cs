using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Models.Travel.Dto
{
    public class UpdateTravelRequest
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateOnly? StartDate { get; set; }

        [Required]
        public DateOnly? EndDate { get; set; }

        [Required]
        [MaxLength(150)]
        public string Destination { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Currency { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal? InitialBudget { get; set; }

        public int? Ranking { get; set; }

        public bool? IsArchived { get; set; }

        public List<TravelParticipantRequest> Participants { get; set; } = new List<TravelParticipantRequest>();
    }
}
