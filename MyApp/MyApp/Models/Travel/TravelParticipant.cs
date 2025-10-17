using System.ComponentModel.DataAnnotations;

namespace MyApp.Models.Travel
{
    public class TravelParticipant
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        public int TravelId { get; set; }

        public Travel? Travel { get; set; }
    }
}
