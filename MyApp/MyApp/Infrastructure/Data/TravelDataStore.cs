using System;
using System.Collections.Generic;
using MyApp.Models.Travel;

namespace MyApp.Infrastructure.Data
{
    public static class TravelDataStore
    {
        public static List<TravelViewModel> GetTravels()
        {
            List<TravelViewModel> travels = new List<TravelViewModel>();

            Guid barcelonaTripId = Guid.Parse("8a25b5d4-0000-4c52-b53f-0fcd7aa1e001");
            List<TravelMilestoneViewModel> barcelonaMilestones = new List<TravelMilestoneViewModel>
            {
                new TravelMilestoneViewModel
                {
                    Id = Guid.Parse("8a25b5d4-0000-4c52-b53f-0fcd7aa1e101"),
                    TravelId = barcelonaTripId,
                    Title = "Vuelo de ida",
                    Date = DateTime.UtcNow.Date.AddDays(10),
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(11, 0, 0),
                    Cost = 320m,
                    Location = "MAD → BCN",
                    Notes = "Equipaje facturado incluido"
                },
                new TravelMilestoneViewModel
                {
                    Id = Guid.Parse("8a25b5d4-0000-4c52-b53f-0fcd7aa1e102"),
                    TravelId = barcelonaTripId,
                    Title = "Tour gastronómico",
                    Date = DateTime.UtcNow.Date.AddDays(12),
                    StartTime = new TimeSpan(12, 30, 0),
                    EndTime = new TimeSpan(15, 30, 0),
                    Cost = 180m,
                    Location = "Barrio Gótico",
                    Notes = "Reservado con guía local"
                },
                new TravelMilestoneViewModel
                {
                    Id = Guid.Parse("8a25b5d4-0000-4c52-b53f-0fcd7aa1e103"),
                    TravelId = barcelonaTripId,
                    Title = "Hotel Boutique",
                    Date = DateTime.UtcNow.Date.AddDays(10),
                    StartTime = new TimeSpan(15, 0, 0),
                    EndTime = new TimeSpan(11, 0, 0),
                    Cost = 540m,
                    Location = "Eixample",
                    Notes = "Tres noches con desayuno"
                }
            };

            travels.Add(new TravelViewModel
            {
                Id = barcelonaTripId,
                Title = "Escapada a Barcelona",
                StartDate = DateTime.UtcNow.Date.AddDays(10),
                EndDate = DateTime.UtcNow.Date.AddDays(13),
                Destination = "Barcelona, España",
                InitialBudget = 1200m,
                Ranking = null,
                IsArchived = false,
                LodgingCostPerNight = 180m,
                Participants = new List<TravelParticipantViewModel>
                {
                    new TravelParticipantViewModel { Email = "ana@example.com", Role = "Organizadora" },
                    new TravelParticipantViewModel { Email = "luis@example.com", Role = "Invitado" }
                },
                Milestones = barcelonaMilestones
            });

            Guid tokyoTripId = Guid.Parse("9b35c6e5-0000-4c52-b53f-0fcd7aa1e002");
            List<TravelMilestoneViewModel> tokyoMilestones = new List<TravelMilestoneViewModel>
            {
                new TravelMilestoneViewModel
                {
                    Id = Guid.Parse("9b35c6e5-0000-4c52-b53f-0fcd7aa1e201"),
                    TravelId = tokyoTripId,
                    Title = "Llegada a Haneda",
                    Date = DateTime.UtcNow.Date.AddDays(-40),
                    StartTime = new TimeSpan(6, 0, 0),
                    EndTime = new TimeSpan(9, 0, 0),
                    Cost = 780m,
                    Location = "HND",
                    Notes = "Vuelo nocturno"
                },
                new TravelMilestoneViewModel
                {
                    Id = Guid.Parse("9b35c6e5-0000-4c52-b53f-0fcd7aa1e202"),
                    TravelId = tokyoTripId,
                    Title = "Visita al teamLab",
                    Date = DateTime.UtcNow.Date.AddDays(-35),
                    StartTime = new TimeSpan(14, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    Cost = 90m,
                    Location = "Odaiba",
                    Notes = "Entradas anticipadas"
                },
                new TravelMilestoneViewModel
                {
                    Id = Guid.Parse("9b35c6e5-0000-4c52-b53f-0fcd7aa1e203"),
                    TravelId = tokyoTripId,
                    Title = "Ryokan tradicional",
                    Date = DateTime.UtcNow.Date.AddDays(-38),
                    StartTime = new TimeSpan(16, 0, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    Cost = 900m,
                    Location = "Hakone",
                    Notes = "Onsen privado"
                }
            };

            travels.Add(new TravelViewModel
            {
                Id = tokyoTripId,
                Title = "Aventura en Tokio",
                StartDate = DateTime.UtcNow.Date.AddDays(-42),
                EndDate = DateTime.UtcNow.Date.AddDays(-32),
                Destination = "Tokio, Japón",
                InitialBudget = 3500m,
                Ranking = 5,
                IsArchived = false,
                LodgingCostPerNight = 220m,
                Participants = new List<TravelParticipantViewModel>
                {
                    new TravelParticipantViewModel { Email = "maria@example.com", Role = "Organizadora" },
                    new TravelParticipantViewModel { Email = "carlos@example.com", Role = "Fotógrafo" }
                },
                Milestones = tokyoMilestones
            });

            Guid patagoniaTripId = Guid.Parse("7c45d7f6-0000-4c52-b53f-0fcd7aa1e003");
            List<TravelMilestoneViewModel> patagoniaMilestones = new List<TravelMilestoneViewModel>
            {
                new TravelMilestoneViewModel
                {
                    Id = Guid.Parse("7c45d7f6-0000-4c52-b53f-0fcd7aa1e301"),
                    TravelId = patagoniaTripId,
                    Title = "Trekking Glaciar",
                    Date = DateTime.UtcNow.Date.AddDays(-200),
                    StartTime = new TimeSpan(7, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    Cost = 260m,
                    Location = "Perito Moreno",
                    Notes = "Guía bilingüe"
                },
                new TravelMilestoneViewModel
                {
                    Id = Guid.Parse("7c45d7f6-0000-4c52-b53f-0fcd7aa1e302"),
                    TravelId = patagoniaTripId,
                    Title = "Cruce en ferry",
                    Date = DateTime.UtcNow.Date.AddDays(-198),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0),
                    Cost = 150m,
                    Location = "Lago Argentino",
                    Notes = "Clima variable"
                }
            };

            travels.Add(new TravelViewModel
            {
                Id = patagoniaTripId,
                Title = "Expedición Patagonia",
                StartDate = DateTime.UtcNow.Date.AddDays(-210),
                EndDate = DateTime.UtcNow.Date.AddDays(-190),
                Destination = "El Calafate, Argentina",
                InitialBudget = 4200m,
                Ranking = 4,
                IsArchived = true,
                LodgingCostPerNight = 160m,
                Participants = new List<TravelParticipantViewModel>
                {
                    new TravelParticipantViewModel { Email = "sofia@example.com", Role = "Guía" }
                },
                Milestones = patagoniaMilestones
            });

            return travels;
        }

        public static TravelViewModel? GetTravel(Guid travelId)
        {
            List<TravelViewModel> travels = GetTravels();
            TravelViewModel? travel = travels.Find(existing => existing.Id == travelId);
            return travel;
        }
    }
}
