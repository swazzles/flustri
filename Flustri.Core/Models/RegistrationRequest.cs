namespace Flustri.Core.Models;

public class RegistrationRequest
{
    public required Guid RegistrationRequestId { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public required string InitialRole { get; set; }

    public bool IsFirstRegistration { get; set; } = false;

    public bool Consumed { get; set; } = false;
    public DateTime? ConsumedAt { get; set; }
    public string? ConsumedByIp { get; set; }
}