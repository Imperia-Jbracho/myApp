using System.Collections.Generic;

namespace MyApp.Models.Travel.ViewModels
{
    public class TravelListViewModel
    {
        public List<TravelListItemViewModel> Travels { get; set; } = new List<TravelListItemViewModel>();

        public string ActiveFilter { get; set; } = "todos";

        public string SearchTerm { get; set; } = string.Empty;

        public int TotalCount { get; set; }

        public int FutureCount { get; set; }

        public int PastCount { get; set; }

        public int ArchivedCount { get; set; }

        public TravelFormViewModel Form { get; set; } = new TravelFormViewModel();
    }
}
