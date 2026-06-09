namespace EventRegistrationsApi.Models;

public class Registration
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..12];
    public string EventId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Active;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime? CancelledAt { get; set; }
}

public enum RegistrationStatus
{
    Active,
    Cancelled
}
