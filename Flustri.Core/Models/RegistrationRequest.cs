namespace Flustri.Core.Models;

public class RegistrationRequest
{
    public required Guid RegistrationRequestId { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public required string InitialRole { get; set; }
}