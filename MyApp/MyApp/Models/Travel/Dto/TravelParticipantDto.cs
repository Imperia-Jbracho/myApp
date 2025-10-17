using System.ComponentModel.DataAnnotations;

namespace MyApp.Models.Travel.Dto
{
    public class TravelParticipantDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
