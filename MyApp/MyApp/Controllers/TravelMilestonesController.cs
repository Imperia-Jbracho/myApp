using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Infrastructure.Data;
using MyApp.Models.Travel;
using MyApp.Models.Travel.Dto;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/travel/{travelId:int}/milestones")]
    public class TravelMilestonesController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public TravelMilestonesController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("{milestoneId:int}")]
        public async Task<ActionResult<TravelMilestoneDto>> GetByIdAsync(int travelId, int milestoneId)
        {
            TravelMilestone? milestone = await context.TravelMilestones
                .AsNoTracking()
                .FirstOrDefaultAsync(entity => entity.TravelId == travelId && entity.Id == milestoneId);

            if (milestone == null)
            {
                return NotFound();
            }

            TravelMilestoneDto dto = MapToDto(milestone);

            return Ok(dto);
        }

        [HttpPost("activities")]
        public async Task<ActionResult<TravelMilestoneDto>> CreateActivityAsync(int travelId, [FromBody] CreateActivityMilestoneRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            Travel? travel = await context.Travels.FirstOrDefaultAsync(entity => entity.Id == travelId);
            if (travel == null)
            {
                return NotFound();
            }

            string titleValue = request.Title ?? string.Empty;
            string trimmedTitle = titleValue.Trim();
            if (string.IsNullOrWhiteSpace(trimmedTitle))
            {
                ModelState.AddModelError(nameof(request.Title), "El título es obligatorio.");
                return ValidationProblem(ModelState);
            }

            if (!request.Date.HasValue)
            {
                ModelState.AddModelError(nameof(request.Date), "La fecha es obligatoria.");
                return ValidationProblem(ModelState);
            }

            DateOnly date = request.Date.Value;
            if (date < travel.StartDate || date > travel.EndDate)
            {
                ModelState.AddModelError(nameof(request.Date), "La fecha debe estar dentro del rango del viaje.");
                return ValidationProblem(ModelState);
            }

            TimeOnly? startTime = request.StartTime;
            TimeOnly? endTime = request.EndTime;
            if (startTime.HasValue && endTime.HasValue && endTime < startTime)
            {
                ModelState.AddModelError(nameof(request.EndTime), "La hora de fin debe ser posterior o igual a la hora de inicio.");
                return ValidationProblem(ModelState);
            }

            double? duration = request.DurationHours;
            if (duration.HasValue && duration.Value < 0d)
            {
                ModelState.AddModelError(nameof(request.DurationHours), "La duración debe ser mayor o igual a cero.");
                return ValidationProblem(ModelState);
            }

            decimal costValue = request.Cost ?? 0m;
            if (costValue < 0m)
            {
                ModelState.AddModelError(nameof(request.Cost), "El coste debe ser mayor o igual a cero.");
                return ValidationProblem(ModelState);
            }

            string classificationValue = request.Classification ?? string.Empty;
            string trimmedClassification = classificationValue.Trim();
            if (trimmedClassification.Length > 200)
            {
                ModelState.AddModelError(nameof(request.Classification), "La clasificación debe tener 200 caracteres o menos.");
                return ValidationProblem(ModelState);
            }

            string locationValue = request.LocationUrl ?? string.Empty;
            string trimmedLocation = locationValue.Trim();
            if (trimmedLocation.Length > 500)
            {
                ModelState.AddModelError(nameof(request.LocationUrl), "La URL debe tener 500 caracteres o menos.");
                return ValidationProblem(ModelState);
            }

            TravelMilestone milestone = new TravelMilestone
            {
                TravelId = travel.Id,
                Type = TravelMilestoneType.Activity,
                Title = trimmedTitle,
                Date = date,
                StartTime = startTime,
                EndTime = endTime,
                DurationHours = duration,
                Classification = trimmedClassification.Length > 0 ? trimmedClassification : null,
                LocationUrl = trimmedLocation.Length > 0 ? trimmedLocation : null,
                Cost = costValue
            };

            context.TravelMilestones.Add(milestone);
            await context.SaveChangesAsync();

            TravelMilestoneDto dto = MapToDto(milestone);

            return CreatedAtAction(nameof(GetByIdAsync), new { travelId, milestoneId = milestone.Id }, dto);
        }

        [HttpPost("lodgings")]
        public async Task<ActionResult<TravelMilestoneDto>> CreateLodgingAsync(int travelId, [FromBody] CreateLodgingMilestoneRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            Travel? travel = await context.Travels.FirstOrDefaultAsync(entity => entity.Id == travelId);
            if (travel == null)
            {
                return NotFound();
            }

            string titleValue = request.Title ?? string.Empty;
            string trimmedTitle = titleValue.Trim();
            if (string.IsNullOrWhiteSpace(trimmedTitle))
            {
                ModelState.AddModelError(nameof(request.Title), "El título es obligatorio.");
                return ValidationProblem(ModelState);
            }

            int? nights = request.Nights;
            if (nights.HasValue && nights.Value < 0)
            {
                ModelState.AddModelError(nameof(request.Nights), "Las noches deben ser mayores o iguales a cero.");
                return ValidationProblem(ModelState);
            }

            DateOnly? checkInDate = request.CheckInDate;
            DateOnly? checkOutDate = request.CheckOutDate;
            if (checkInDate.HasValue && (checkInDate.Value < travel.StartDate || checkInDate.Value > travel.EndDate))
            {
                ModelState.AddModelError(nameof(request.CheckInDate), "El check-in debe estar dentro del rango del viaje.");
                return ValidationProblem(ModelState);
            }

            if (checkOutDate.HasValue && (checkOutDate.Value < travel.StartDate || checkOutDate.Value > travel.EndDate))
            {
                ModelState.AddModelError(nameof(request.CheckOutDate), "El check-out debe estar dentro del rango del viaje.");
                return ValidationProblem(ModelState);
            }

            if (checkInDate.HasValue && checkOutDate.HasValue && checkOutDate.Value < checkInDate.Value)
            {
                ModelState.AddModelError(nameof(request.CheckOutDate), "El check-out debe ser posterior o igual al check-in.");
                return ValidationProblem(ModelState);
            }

            decimal costValue = request.Cost ?? 0m;
            if (costValue < 0m)
            {
                ModelState.AddModelError(nameof(request.Cost), "El coste debe ser mayor o igual a cero.");
                return ValidationProblem(ModelState);
            }

            string locationValue = request.LocationUrl ?? string.Empty;
            string trimmedLocation = locationValue.Trim();
            if (trimmedLocation.Length > 500)
            {
                ModelState.AddModelError(nameof(request.LocationUrl), "La URL debe tener 500 caracteres o menos.");
                return ValidationProblem(ModelState);
            }

            string bookingValue = request.BookingPlatform ?? string.Empty;
            string trimmedBooking = bookingValue.Trim();
            if (trimmedBooking.Length > 150)
            {
                ModelState.AddModelError(nameof(request.BookingPlatform), "La plataforma debe tener 150 caracteres o menos.");
                return ValidationProblem(ModelState);
            }

            DateOnly lodgingDate = checkInDate ?? checkOutDate ?? travel.StartDate;

            decimal? nightlyRate = null;
            if (nights.HasValue && nights.Value > 0)
            {
                nightlyRate = Math.Round(costValue / nights.Value, 2, MidpointRounding.AwayFromZero);
            }

            TravelMilestone milestone = new TravelMilestone
            {
                TravelId = travel.Id,
                Type = TravelMilestoneType.Lodging,
                Title = trimmedTitle,
                Date = lodgingDate,
                Nights = nights,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                Cost = costValue,
                NightlyRate = nightlyRate,
                BookingPlatform = trimmedBooking.Length > 0 ? trimmedBooking : null,
                LocationUrl = trimmedLocation.Length > 0 ? trimmedLocation : null
            };

            context.TravelMilestones.Add(milestone);
            await context.SaveChangesAsync();

            TravelMilestoneDto dto = MapToDto(milestone);

            return CreatedAtAction(nameof(GetByIdAsync), new { travelId, milestoneId = milestone.Id }, dto);
        }

        [HttpPost("restaurants")]
        public async Task<ActionResult<TravelMilestoneDto>> CreateRestaurantAsync(int travelId, [FromBody] CreateRestaurantMilestoneRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            Travel? travel = await context.Travels.FirstOrDefaultAsync(entity => entity.Id == travelId);
            if (travel == null)
            {
                return NotFound();
            }

            string titleValue = request.Title ?? string.Empty;
            string trimmedTitle = titleValue.Trim();
            if (string.IsNullOrWhiteSpace(trimmedTitle))
            {
                ModelState.AddModelError(nameof(request.Title), "El nombre del restaurante es obligatorio.");
                return ValidationProblem(ModelState);
            }

            if (!request.ReservationDate.HasValue)
            {
                ModelState.AddModelError(nameof(request.ReservationDate), "La fecha de la reserva es obligatoria.");
                return ValidationProblem(ModelState);
            }

            DateOnly reservationDate = request.ReservationDate.Value;
            if (reservationDate < travel.StartDate || reservationDate > travel.EndDate)
            {
                ModelState.AddModelError(nameof(request.ReservationDate), "La fecha debe estar dentro del rango del viaje.");
                return ValidationProblem(ModelState);
            }

            decimal costValue = request.Cost ?? 0m;
            if (costValue < 0m)
            {
                ModelState.AddModelError(nameof(request.Cost), "El gasto estimado debe ser mayor o igual a cero.");
                return ValidationProblem(ModelState);
            }

            string locationValue = request.LocationUrl ?? string.Empty;
            string trimmedLocation = locationValue.Trim();
            if (trimmedLocation.Length > 500)
            {
                ModelState.AddModelError(nameof(request.LocationUrl), "La URL debe tener 500 caracteres o menos.");
                return ValidationProblem(ModelState);
            }

            TravelMilestone milestone = new TravelMilestone
            {
                TravelId = travel.Id,
                Type = TravelMilestoneType.Restaurant,
                Title = trimmedTitle,
                Date = reservationDate,
                ReservationDate = reservationDate,
                StartTime = request.StartTime,
                Cost = costValue,
                LocationUrl = trimmedLocation.Length > 0 ? trimmedLocation : null
            };

            context.TravelMilestones.Add(milestone);
            await context.SaveChangesAsync();

            TravelMilestoneDto dto = MapToDto(milestone);

            return CreatedAtAction(nameof(GetByIdAsync), new { travelId, milestoneId = milestone.Id }, dto);
        }

        private static TravelMilestoneDto MapToDto(TravelMilestone milestone)
        {
            TravelMilestoneDto dto = new TravelMilestoneDto
            {
                Id = milestone.Id,
                TravelId = milestone.TravelId,
                Type = milestone.Type,
                Title = milestone.Title,
                Date = milestone.Date,
                StartTime = milestone.StartTime,
                EndTime = milestone.EndTime,
                DurationHours = milestone.DurationHours,
                Classification = milestone.Classification,
                LocationUrl = milestone.LocationUrl,
                Cost = milestone.Cost,
                Nights = milestone.Nights,
                CheckInDate = milestone.CheckInDate,
                CheckOutDate = milestone.CheckOutDate,
                NightlyRate = milestone.NightlyRate,
                BookingPlatform = milestone.BookingPlatform,
                ReservationDate = milestone.ReservationDate
            };

            return dto;
        }
    }
}
