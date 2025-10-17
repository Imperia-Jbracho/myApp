using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [MaxLength(10)]
        public string Currency { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int DurationDays { get; set; }

        public ICollection<TravelParticipant> Participants { get; set; } = new List<TravelParticipant>();
    }
}
