using System.ComponentModel.DataAnnotations;

namespace EventRegistrationsApi.Models;

public class CreateEventRequest
{
    [Required(ErrorMessage = "Event name is required.")]
    [MinLength(1, ErrorMessage = "Event name cannot be empty.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "total_seats is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "total_seats must be a positive integer greater than 0.")]
    public int TotalSeats { get; set; }

    [Required(ErrorMessage = "event_date is required (ISO 8601 format).")]
    public DateTime EventDate { get; set; }
}

public class CreateRegistrationRequest
{
    [Required(ErrorMessage = "user_name is required.")]
    [MinLength(1, ErrorMessage = "user_name cannot be empty.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "event_id is required.")]
    public string EventId { get; set; } = string.Empty;
}

public class EventResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public DateTime EventDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalRegistrations { get; set; }
}

public class EventListResponse
{
    public int Count { get; set; }
    public List<EventResponse> Events { get; set; } = [];
}

public class RegistrationResponse
{
    public string Id { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}

public class RegistrationListResponse
{
    public int Count { get; set; }
    public List<RegistrationResponse> Registrations { get; set; } = [];
}

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
}

public class MessageResponse
{
    public string Message { get; set; } = string.Empty;
}
