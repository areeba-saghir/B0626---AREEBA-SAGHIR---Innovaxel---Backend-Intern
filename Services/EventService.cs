using EventRegistrationsApi.Data;
using EventRegistrationsApi.Models;

namespace EventRegistrationsApi.Services;

public class EventService
{
    private readonly JsonDataStore _store;

    public EventService(JsonDataStore store) => _store = store;


    public async Task<ServiceResult<EventResponse>> CreateAsync(CreateEventRequest req)
    {
        if (req.EventDate.ToUniversalTime() <= DateTime.UtcNow)
            return ServiceResult<EventResponse>.Fail("event_date must be in the future.");

        return await _store.ExecuteAsync(store =>
        {
            bool duplicate = store.Events.Any(
                e => e.Name.Equals(req.Name.Trim(), StringComparison.OrdinalIgnoreCase));

            if (duplicate)
                return ServiceResult<EventResponse>.Fail(
                    "An event with this name already exists.", 409);

            var ev = new Event
            {
                Name = req.Name.Trim(),
                TotalSeats = req.TotalSeats,
                AvailableSeats = req.TotalSeats,
                EventDate = req.EventDate.ToUniversalTime(),
            };

            store.Events.Add(ev);
            return ServiceResult<EventResponse>.Created(ToResponse(ev, 0));
        });
    }


    public async Task<EventListResponse> ListAsync(bool upcomingOnly, string sort)
    {
        return await _store.QueryAsync(store =>
        {
            var now = DateTime.UtcNow;
            var events = store.Events.AsEnumerable();

            if (upcomingOnly)
                events = events.Where(e => e.EventDate > now);

            events = sort?.ToLower() == "desc"
                ? events.OrderByDescending(e => e.EventDate)
                : events.OrderBy(e => e.EventDate);

            var list = events.Select(e =>
            {
                int regCount = store.Registrations.Count(
                    r => r.EventId == e.Id && r.Status == RegistrationStatus.Active);
                return ToResponse(e, regCount);
            }).ToList();

            return new EventListResponse { Count = list.Count, Events = list };
        });
    }

    public async Task<ServiceResult<EventResponse>> GetByIdAsync(string id)
    {
        return await _store.QueryAsync(store =>
        {
            var ev = store.Events.FirstOrDefault(e => e.Id == id);
            if (ev is null)
                return ServiceResult<EventResponse>.Fail("Event not found.", 404);

            int regCount = store.Registrations.Count(
                r => r.EventId == id && r.Status == RegistrationStatus.Active);

            return ServiceResult<EventResponse>.Ok(ToResponse(ev, regCount));
        });
    }

    private static EventResponse ToResponse(Event e, int regCount) => new()
    {
        Id = e.Id,
        Name = e.Name,
        TotalSeats = e.TotalSeats,
        AvailableSeats = e.AvailableSeats,
        EventDate = e.EventDate,
        CreatedAt = e.CreatedAt,
        TotalRegistrations = regCount,
    };
}
