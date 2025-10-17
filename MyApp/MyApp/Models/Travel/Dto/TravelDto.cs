using System.Collections.Generic;

namespace MyApp.Models.Travel.Dto
{
    public class TravelDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string Currency { get; set; } = string.Empty;

        public int DurationDays { get; set; }

        public List<string> Participants { get; set; } = new List<string>();
    }
}
