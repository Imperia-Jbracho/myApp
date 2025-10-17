using System.Collections.Generic;

namespace MyApp.Models.Travel
{
    public class TravelDashboardViewModel
    {
        public List<TravelViewModel> Travels { get; set; } = new List<TravelViewModel>();

        public string SelectedFilter { get; set; } = string.Empty;

        public string SearchTerm { get; set; } = string.Empty;
    }
}
