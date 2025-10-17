using System.Collections.Generic;

namespace MyApp.Models.Travel.Dto
{
    public class TravelDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string Currency { get; set; } = string.Empty;

        public int DurationDays { get; set; }

        public decimal InitialBudget { get; set; }

        public decimal TotalCost { get; set; }

        public int? Ranking { get; set; }

        public bool IsArchived { get; set; }

        public List<TravelParticipantDto> Participants { get; set; } = new List<TravelParticipantDto>();
    }
}
