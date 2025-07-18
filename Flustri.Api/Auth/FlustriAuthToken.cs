namespace Flustri.Api.Auth;

public record FlustriAuthToken (
    string UserId,
    byte[] Signature
);