namespace Flustri.Api.Auth;

public record FlustriAuthToken (
    Guid UserId,
    byte[] Signature
);