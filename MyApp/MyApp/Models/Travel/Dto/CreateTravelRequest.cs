using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Models.Travel.Dto
{
    public class CreateTravelRequest
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateOnly? StartDate { get; set; }

        [Required]
        public DateOnly? EndDate { get; set; }

        [Required]
        [MaxLength(10)]
        public string Currency { get; set; } = string.Empty;

        public List<string> Participants { get; set; } = new List<string>();
    }
}
