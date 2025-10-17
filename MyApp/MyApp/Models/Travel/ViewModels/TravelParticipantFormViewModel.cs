using System.ComponentModel.DataAnnotations;

namespace MyApp.Models.Travel.ViewModels
{
    public class TravelParticipantFormViewModel
    {
        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Role { get; set; } = string.Empty;
    }
}
