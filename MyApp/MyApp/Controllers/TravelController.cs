using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Infrastructure.Data;
using MyApp.Models.Travel;
using MyApp.Models.Travel.Dto;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public TravelController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<TravelDto>>> GetAllAsync()
        {
            List<Travel> travels = await context.Travels
                .Include(travel => travel.Participants)
                .ToListAsync();

            List<TravelDto> response = travels
                .Select(MapToDto)
                .ToList();

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TravelDto>> GetByIdAsync(int id)
        {
            Travel? travel = await context.Travels
                .Include(entity => entity.Participants)
                .FirstOrDefaultAsync(entity => entity.Id == id);

            if (travel == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(travel));
        }

        [HttpPost]
        public async Task<ActionResult<TravelDto>> CreateAsync([FromBody] CreateTravelRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            string titleValue = request.Title ?? string.Empty;
            string trimmedTitle = titleValue.Trim();
            if (string.IsNullOrWhiteSpace(trimmedTitle))
            {
                ModelState.AddModelError(nameof(request.Title), "El título es obligatorio.");
                return ValidationProblem(ModelState);
            }

            if (!request.StartDate.HasValue || !request.EndDate.HasValue)
            {
                ModelState.AddModelError(nameof(request.StartDate), "Las fechas de inicio y fin son obligatorias.");
                return ValidationProblem(ModelState);
            }

            if (request.EndDate.Value < request.StartDate.Value)
            {
                ModelState.AddModelError(nameof(request.EndDate), "La fecha de fin debe ser mayor o igual a la fecha de inicio.");
                return ValidationProblem(ModelState);
            }

            string currencyValue = request.Currency ?? string.Empty;
            string normalizedCurrency = currencyValue.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(normalizedCurrency))
            {
                ModelState.AddModelError(nameof(request.Currency), "La moneda es obligatoria.");
                return ValidationProblem(ModelState);
            }

            List<string> participantErrors = ValidateParticipants(request.Participants);
            if (participantErrors.Count > 0)
            {
                foreach (string error in participantErrors)
                {
                    ModelState.AddModelError(nameof(request.Participants), error);
                }

                return ValidationProblem(ModelState);
            }

            Travel travel = new Travel
            {
                Title = trimmedTitle,
                StartDate = request.StartDate.Value,
                EndDate = request.EndDate.Value,
                Currency = normalizedCurrency,
                DurationDays = CalculateDurationDays(request.StartDate.Value, request.EndDate.Value)
            };

            travel.Participants = NormalizeParticipants(request.Participants)
                .Select(email => new TravelParticipant
                {
                    Email = email
                })
                .ToList();

            context.Travels.Add(travel);
            await context.SaveChangesAsync();

            TravelDto created = MapToDto(travel);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = travel.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TravelDto>> UpdateAsync(int id, [FromBody] UpdateTravelRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            string titleValue = request.Title ?? string.Empty;
            string trimmedTitle = titleValue.Trim();
            if (string.IsNullOrWhiteSpace(trimmedTitle))
            {
                ModelState.AddModelError(nameof(request.Title), "El título es obligatorio.");
                return ValidationProblem(ModelState);
            }

            if (!request.StartDate.HasValue || !request.EndDate.HasValue)
            {
                ModelState.AddModelError(nameof(request.StartDate), "Las fechas de inicio y fin son obligatorias.");
                return ValidationProblem(ModelState);
            }

            if (request.EndDate.Value < request.StartDate.Value)
            {
                ModelState.AddModelError(nameof(request.EndDate), "La fecha de fin debe ser mayor o igual a la fecha de inicio.");
                return ValidationProblem(ModelState);
            }

            string currencyValue = request.Currency ?? string.Empty;
            string normalizedCurrency = currencyValue.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(normalizedCurrency))
            {
                ModelState.AddModelError(nameof(request.Currency), "La moneda es obligatoria.");
                return ValidationProblem(ModelState);
            }

            Travel? travel = await context.Travels
                .Include(entity => entity.Participants)
                .FirstOrDefaultAsync(entity => entity.Id == id);

            if (travel == null)
            {
                return NotFound();
            }

            List<string> participantErrors = ValidateParticipants(request.Participants);
            if (participantErrors.Count > 0)
            {
                foreach (string error in participantErrors)
                {
                    ModelState.AddModelError(nameof(request.Participants), error);
                }

                return ValidationProblem(ModelState);
            }

            travel.Title = trimmedTitle;
            travel.StartDate = request.StartDate.Value;
            travel.EndDate = request.EndDate.Value;
            travel.Currency = normalizedCurrency;
            travel.DurationDays = CalculateDurationDays(request.StartDate.Value, request.EndDate.Value);

            UpdateParticipants(travel, request.Participants);

            await context.SaveChangesAsync();

            return Ok(MapToDto(travel));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            Travel? travel = await context.Travels
                .Include(entity => entity.Participants)
                .FirstOrDefaultAsync(entity => entity.Id == id);

            if (travel == null)
            {
                return NotFound();
            }

            context.Travels.Remove(travel);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private static TravelDto MapToDto(Travel travel)
        {
            return new TravelDto
            {
                Id = travel.Id,
                Title = travel.Title,
                StartDate = travel.StartDate,
                EndDate = travel.EndDate,
                Currency = travel.Currency,
                DurationDays = travel.DurationDays,
                Participants = travel.Participants
                    .Select(participant => participant.Email)
                    .OrderBy(email => email)
                    .ToList()
            };
        }

        private static int CalculateDurationDays(DateOnly startDate, DateOnly endDate)
        {
            int days = endDate.DayNumber - startDate.DayNumber + 1;

            return days;
        }

        private static List<string> ValidateParticipants(List<string>? participants)
        {
            List<string> errors = new List<string>();

            if (participants == null)
            {
                return errors;
            }

            EmailAddressAttribute attribute = new EmailAddressAttribute();

            for (int index = 0; index < participants.Count; index++)
            {
                string email = participants[index];
                if (string.IsNullOrWhiteSpace(email))
                {
                    errors.Add($"El participante en la posición {index + 1} debe contener un correo electrónico válido.");
                    continue;
                }

                if (!attribute.IsValid(email))
                {
                    errors.Add($"El correo '{email}' no es válido.");
                }
            }

            return errors;
        }

        private static List<string> NormalizeParticipants(List<string>? participants)
        {
            if (participants == null)
            {
                return new List<string>();
            }

            List<string> normalizedParticipants = participants
                .Where(email => !string.IsNullOrWhiteSpace(email))
                .Select(email => email.Trim().ToLowerInvariant())
                .Distinct()
                .ToList();

            return normalizedParticipants;
        }

        private static void UpdateParticipants(Travel travel, List<string>? participants)
        {
            List<string> normalizedParticipants = NormalizeParticipants(participants);

            List<TravelParticipant> toRemove = travel.Participants
                .Where(participant => !normalizedParticipants.Contains(participant.Email))
                .ToList();

            foreach (TravelParticipant participant in toRemove)
            {
                travel.Participants.Remove(participant);
            }

            foreach (string email in normalizedParticipants)
            {
                bool exists = travel.Participants.Any(participant => participant.Email == email);
                if (!exists)
                {
                    travel.Participants.Add(new TravelParticipant
                    {
                        Email = email
                    });
                }
            }
        }
    }
}
