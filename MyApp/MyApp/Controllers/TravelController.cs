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
                .Include(travel => travel.Milestones)
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
                .Include(entity => entity.Milestones)
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

            DateOnly startDate = request.StartDate.Value;
            DateOnly endDate = request.EndDate.Value;
            if (endDate < startDate)
            {
                ModelState.AddModelError(nameof(request.EndDate), "La fecha de fin debe ser mayor o igual a la fecha de inicio.");
                return ValidationProblem(ModelState);
            }

            string destinationValue = request.Destination ?? string.Empty;
            string trimmedDestination = destinationValue.Trim();
            if (string.IsNullOrWhiteSpace(trimmedDestination))
            {
                ModelState.AddModelError(nameof(request.Destination), "El destino es obligatorio.");
                return ValidationProblem(ModelState);
            }

            string currencyValue = request.Currency ?? string.Empty;
            string normalizedCurrency = currencyValue.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(normalizedCurrency))
            {
                ModelState.AddModelError(nameof(request.Currency), "La moneda es obligatoria.");
                return ValidationProblem(ModelState);
            }

            decimal initialBudgetValue = request.InitialBudget ?? 0m;
            if (initialBudgetValue < 0m)
            {
                ModelState.AddModelError(nameof(request.InitialBudget), "El presupuesto debe ser mayor o igual a cero.");
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

            List<TravelParticipantRequest> normalizedParticipants = NormalizeParticipants(request.Participants);

            Travel travel = new Travel
            {
                Title = trimmedTitle,
                StartDate = startDate,
                EndDate = endDate,
                Destination = trimmedDestination,
                Currency = normalizedCurrency,
                InitialBudget = initialBudgetValue,
                DurationDays = CalculateDurationDays(startDate, endDate),
                Ranking = null,
                IsArchived = false
            };

            List<TravelParticipant> participants = normalizedParticipants
                .Select(participant => new TravelParticipant
                {
                    Email = participant.Email,
                    Role = participant.Role
                })
                .ToList();

            travel.Participants = participants;

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

            DateOnly startDate = request.StartDate.Value;
            DateOnly endDate = request.EndDate.Value;
            if (endDate < startDate)
            {
                ModelState.AddModelError(nameof(request.EndDate), "La fecha de fin debe ser mayor o igual a la fecha de inicio.");
                return ValidationProblem(ModelState);
            }

            string destinationValue = request.Destination ?? string.Empty;
            string trimmedDestination = destinationValue.Trim();
            if (string.IsNullOrWhiteSpace(trimmedDestination))
            {
                ModelState.AddModelError(nameof(request.Destination), "El destino es obligatorio.");
                return ValidationProblem(ModelState);
            }

            string currencyValue = request.Currency ?? string.Empty;
            string normalizedCurrency = currencyValue.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(normalizedCurrency))
            {
                ModelState.AddModelError(nameof(request.Currency), "La moneda es obligatoria.");
                return ValidationProblem(ModelState);
            }

            decimal initialBudgetValue = request.InitialBudget ?? 0m;
            if (initialBudgetValue < 0m)
            {
                ModelState.AddModelError(nameof(request.InitialBudget), "El presupuesto debe ser mayor o igual a cero.");
                return ValidationProblem(ModelState);
            }

            if (request.Ranking.HasValue && (request.Ranking.Value < 0 || request.Ranking.Value > 5))
            {
                ModelState.AddModelError(nameof(request.Ranking), "El ranking debe estar entre 0 y 5.");
                return ValidationProblem(ModelState);
            }

            Travel? travel = await context.Travels
                .Include(entity => entity.Participants)
                .Include(entity => entity.Milestones)
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
            travel.StartDate = startDate;
            travel.EndDate = endDate;
            travel.Destination = trimmedDestination;
            travel.Currency = normalizedCurrency;
            travel.InitialBudget = initialBudgetValue;
            travel.DurationDays = CalculateDurationDays(startDate, endDate);

            if (request.Ranking.HasValue)
            {
                travel.Ranking = request.Ranking.Value;
            }

            if (request.IsArchived.HasValue)
            {
                travel.IsArchived = request.IsArchived.Value;
            }

            UpdateParticipants(travel, request.Participants);

            await context.SaveChangesAsync();

            TravelDto updated = MapToDto(travel);

            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            Travel? travel = await context.Travels
                .Include(entity => entity.Participants)
                .Include(entity => entity.Milestones)
                .FirstOrDefaultAsync(entity => entity.Id == id);

            if (travel == null)
            {
                return NotFound();
            }

            context.Travels.Remove(travel);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id:int}/archive")]
        public async Task<ActionResult<TravelDto>> ArchiveAsync(int id)
        {
            Travel? travel = await context.Travels
                .Include(entity => entity.Participants)
                .Include(entity => entity.Milestones)
                .FirstOrDefaultAsync(entity => entity.Id == id);

            if (travel == null)
            {
                return NotFound();
            }

            travel.IsArchived = true;

            await context.SaveChangesAsync();

            TravelDto updated = MapToDto(travel);

            return Ok(updated);
        }

        [HttpPost("{id:int}/unarchive")]
        public async Task<ActionResult<TravelDto>> UnarchiveAsync(int id)
        {
            Travel? travel = await context.Travels
                .Include(entity => entity.Participants)
                .Include(entity => entity.Milestones)
                .FirstOrDefaultAsync(entity => entity.Id == id);

            if (travel == null)
            {
                return NotFound();
            }

            travel.IsArchived = false;

            await context.SaveChangesAsync();

            TravelDto updated = MapToDto(travel);

            return Ok(updated);
        }

        [HttpPost("{id:int}/duplicate")]
        public async Task<ActionResult<TravelDto>> DuplicateAsync(int id)
        {
            Travel? travel = await context.Travels
                .Include(entity => entity.Participants)
                .Include(entity => entity.Milestones)
                .FirstOrDefaultAsync(entity => entity.Id == id);

            if (travel == null)
            {
                return NotFound();
            }

            Travel duplicate = new Travel
            {
                Title = string.Concat(travel.Title, " (Copia)"),
                StartDate = travel.StartDate,
                EndDate = travel.EndDate,
                Destination = travel.Destination,
                Currency = travel.Currency,
                InitialBudget = travel.InitialBudget,
                DurationDays = CalculateDurationDays(travel.StartDate, travel.EndDate),
                Ranking = null,
                IsArchived = false
            };

            List<TravelParticipant> participantCopies = travel.Participants
                .Select(participant => new TravelParticipant
                {
                    Email = participant.Email,
                    Role = participant.Role
                })
                .ToList();

            List<TravelMilestone> milestoneCopies = travel.Milestones
                .Select(milestone => new TravelMilestone
                {
                    Date = milestone.Date,
                    StartTime = milestone.StartTime,
                    EndTime = milestone.EndTime,
                    Title = milestone.Title,
                    Cost = milestone.Cost
                })
                .ToList();

            duplicate.Participants = participantCopies;
            duplicate.Milestones = milestoneCopies;

            context.Travels.Add(duplicate);
            await context.SaveChangesAsync();

            TravelDto created = MapToDto(duplicate);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = duplicate.Id }, created);
        }

        private static TravelDto MapToDto(Travel travel)
        {
            decimal totalCost = travel.Milestones
                .Sum(milestone => milestone.Cost);

            List<TravelParticipantDto> participants = travel.Participants
                .OrderBy(participant => participant.Email)
                .Select(participant => new TravelParticipantDto
                {
                    Email = participant.Email,
                    Role = participant.Role
                })
                .ToList();

            TravelDto dto = new TravelDto
            {
                Id = travel.Id,
                Title = travel.Title,
                Destination = travel.Destination,
                StartDate = travel.StartDate,
                EndDate = travel.EndDate,
                Currency = travel.Currency,
                DurationDays = travel.DurationDays,
                InitialBudget = travel.InitialBudget,
                TotalCost = totalCost,
                Ranking = travel.Ranking,
                IsArchived = travel.IsArchived,
                Participants = participants
            };

            return dto;
        }

        private static int CalculateDurationDays(DateOnly startDate, DateOnly endDate)
        {
            int days = endDate.DayNumber - startDate.DayNumber + 1;

            return days;
        }

        private static List<string> ValidateParticipants(List<TravelParticipantRequest>? participants)
        {
            List<string> errors = new List<string>();

            if (participants == null)
            {
                return errors;
            }

            EmailAddressAttribute attribute = new EmailAddressAttribute();
            HashSet<string> seenEmails = new HashSet<string>();

            for (int index = 0; index < participants.Count; index++)
            {
                TravelParticipantRequest? participant = participants[index];
                if (participant == null)
                {
                    errors.Add($"El participante en la posición {index + 1} es inválido.");
                    continue;
                }

                string emailValue = participant.Email ?? string.Empty;
                string trimmedEmail = emailValue.Trim();

                if (string.IsNullOrWhiteSpace(trimmedEmail))
                {
                    errors.Add($"El participante en la posición {index + 1} debe contener un correo electrónico válido.");
                    continue;
                }

                if (!attribute.IsValid(trimmedEmail))
                {
                    errors.Add($"El correo '{trimmedEmail}' no es válido.");
                }

                string normalizedEmail = trimmedEmail.ToLowerInvariant();
                if (seenEmails.Contains(normalizedEmail))
                {
                    errors.Add($"El correo '{trimmedEmail}' está duplicado.");
                }
                else
                {
                    seenEmails.Add(normalizedEmail);
                }

                string roleValue = participant.Role ?? string.Empty;
                if (roleValue.Length > 100)
                {
                    errors.Add($"El rol del participante '{trimmedEmail}' debe tener 100 caracteres o menos.");
                }
            }

            return errors;
        }

        private static List<TravelParticipantRequest> NormalizeParticipants(List<TravelParticipantRequest>? participants)
        {
            List<TravelParticipantRequest> normalized = new List<TravelParticipantRequest>();

            if (participants == null)
            {
                return normalized;
            }

            Dictionary<string, TravelParticipantRequest> byEmail = new Dictionary<string, TravelParticipantRequest>();

            foreach (TravelParticipantRequest participant in participants)
            {
                if (participant == null)
                {
                    continue;
                }

                string emailValue = participant.Email ?? string.Empty;
                string trimmedEmail = emailValue.Trim().ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(trimmedEmail))
                {
                    continue;
                }

                string roleValue = participant.Role ?? string.Empty;
                string trimmedRole = roleValue.Trim();

                TravelParticipantRequest normalizedParticipant = new TravelParticipantRequest
                {
                    Email = trimmedEmail,
                    Role = trimmedRole
                };

                byEmail[trimmedEmail] = normalizedParticipant;
            }

            List<TravelParticipantRequest> orderedParticipants = byEmail
                .OrderBy(entry => entry.Key)
                .Select(entry => entry.Value)
                .ToList();

            return orderedParticipants;
        }

        private static void UpdateParticipants(Travel travel, List<TravelParticipantRequest>? participants)
        {
            List<TravelParticipantRequest> normalizedParticipants = NormalizeParticipants(participants);

            List<TravelParticipant> toRemove = travel.Participants
                .Where(participant => !normalizedParticipants.Any(requestParticipant => requestParticipant.Email == participant.Email))
                .ToList();

            foreach (TravelParticipant participant in toRemove)
            {
                travel.Participants.Remove(participant);
            }

            foreach (TravelParticipantRequest requestParticipant in normalizedParticipants)
            {
                TravelParticipant? existing = travel.Participants
                    .FirstOrDefault(participant => participant.Email == requestParticipant.Email);

                if (existing == null)
                {
                    TravelParticipant newParticipant = new TravelParticipant
                    {
                        Email = requestParticipant.Email,
                        Role = requestParticipant.Role
                    };

                    travel.Participants.Add(newParticipant);
                }
                else
                {
                    existing.Role = requestParticipant.Role;
                }
            }
        }
    }
}
