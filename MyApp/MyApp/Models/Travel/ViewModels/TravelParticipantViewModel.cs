using System.ComponentModel.DataAnnotations;

namespace MyApp.Models.Travel.ViewModels
{
    public class TravelParticipantViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Role { get; set; } = string.Empty;
    }
}
