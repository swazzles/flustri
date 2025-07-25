using System.Text;
using System.Text.Unicode;
using Flustri.Core.Models;

namespace Flustri.Core.Commands;

public record RegisterCommandRequest (
    Guid RequestId,
    byte[] PublicKey,
    byte[] Signature,
    string Nickname
);

public static class RegisterCommand
{

    public static async Task ConsumeAsync(RegisterCommandRequest request, ISigningService signingService, FlustriDbContext db)
    {
        var registrationRequest = db.RegistrationRequests.FirstOrDefault(r => r.RegistrationRequestId == request.RequestId);

        if (registrationRequest is null)
            throw new Exception("Registration request is expired or invalid.");

        if (registrationRequest.ExpiresAt >= DateTime.UtcNow)
        {
            db.RegistrationRequests.Remove(registrationRequest);
            await db.SaveChangesAsync();
            throw new Exception("Registration request is expired or invalid.");
        }

        var valid = signingService.Verify(request.PublicKey, request.RequestId.ToByteArray(), request.Signature);
        if (!valid)
            throw new Exception("Registration request is expired or invalid.");

        var newUser = new User()
        {
            UserId = Guid.NewGuid(),
            Nickname = request.Nickname,
            PublicKey = request.PublicKey,
            Version = Guid.NewGuid()
        };

        await db.Users.AddAsync(newUser);
        db.RegistrationRequests.Remove(registrationRequest);
        await db.SaveChangesAsync();
    }
}