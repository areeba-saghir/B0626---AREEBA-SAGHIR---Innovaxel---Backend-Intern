using EventRegistrationsApi.Data;
using EventRegistrationsApi.Models;

namespace EventRegistrationsApi.Services;

public class RegistrationService
{
    private readonly JsonDataStore _store;

    public RegistrationService(JsonDataStore store) => _store = store;
    public async Task<ServiceResult<RegistrationResponse>> RegisterAsync(CreateRegistrationRequest req)
    {
        return await _store.ExecuteAsync(store =>
        {
            var ev = store.Events.FirstOrDefault(e => e.Id == req.EventId);
            if (ev is null)
                return ServiceResult<RegistrationResponse>.Fail("Event not found.", 404);
            bool alreadyRegistered = store.Registrations.Any(r =>
                r.EventId == req.EventId &&
                r.UserName.Equals(req.UserName.Trim(), StringComparison.OrdinalIgnoreCase) &&
                r.Status == RegistrationStatus.Active);

            if (alreadyRegistered)
                return ServiceResult<RegistrationResponse>.Fail(
                    "This user is already registered for the event.", 409);


            if (ev.AvailableSeats <= 0)
                return ServiceResult<RegistrationResponse>.Fail(
                    "No seats available. The event is full.", 409);

            ev.AvailableSeats--;

            var reg = new Registration
            {
                EventId = req.EventId,
                UserName = req.UserName.Trim(),
            };
            store.Registrations.Add(reg);

            return ServiceResult<RegistrationResponse>.Created(ToResponse(reg, ev.Name));
        });
    }


    public async Task<ServiceResult<RegistrationResponse>> CancelAsync(string id)
    {
        return await _store.ExecuteAsync(store =>
        {
            var reg = store.Registrations.FirstOrDefault(r => r.Id == id);
            if (reg is null)
                return ServiceResult<RegistrationResponse>.Fail("Registration not found.", 404);

            if (reg.Status == RegistrationStatus.Cancelled)
                return ServiceResult<RegistrationResponse>.Fail(
                    "Registration is already cancelled.", 409);

            reg.Status = RegistrationStatus.Cancelled;
            reg.CancelledAt = DateTime.UtcNow;

            var ev = store.Events.FirstOrDefault(e => e.Id == reg.EventId);
            if (ev is not null)
                ev.AvailableSeats = Math.Min(ev.AvailableSeats + 1, ev.TotalSeats);

            return ServiceResult<RegistrationResponse>.Ok(ToResponse(reg, ev?.Name ?? string.Empty));
        });
    }


    public async Task<RegistrationListResponse> ListAsync(string? eventId, string? status)
    {
        return await _store.QueryAsync(store =>
        {
            var regs = store.Registrations.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(eventId))
                regs = regs.Where(r => r.EventId == eventId);


            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<RegistrationStatus>(status, ignoreCase: true, out var parsed))
                regs = regs.Where(r => r.Status == parsed);
            else
                regs = regs.Where(r => r.Status == RegistrationStatus.Active);

            var eventLookup = store.Events.ToDictionary(e => e.Id, e => e.Name);

            var list = regs.Select(r =>
                ToResponse(r, eventLookup.GetValueOrDefault(r.EventId, string.Empty))
            ).ToList();

            return new RegistrationListResponse { Count = list.Count, Registrations = list };
        });
    }


    private static RegistrationResponse ToResponse(Registration r, string eventName) => new()
    {
        Id = r.Id,
        EventId = r.EventId,
        EventName = eventName,
        UserName = r.UserName,
        Status = r.Status.ToString(),
        RegisteredAt = r.RegisteredAt,
        CancelledAt = r.CancelledAt,
    };
}
