using System.Net;
using Flustri.Core.Models;
using Flustri.Core.Services;
using NSec.Cryptography;

namespace Flustri.Core.Commands;

public record RegisterCommandRequest (
    Guid RequestId,
    PublicKey PublicKey,
    byte[] Signature,
    string Nickname
);

public static class RegisterCommand
{

    public static async Task ConsumeAsync(RegisterCommandRequest request, ISigningService signingService, ILocksmithService locksmithService, FlustriDbContext db, IPAddress ip)
    {
        var registrationRequest = db.RegistrationRequests.FirstOrDefault(r => r.RegistrationRequestId == request.RequestId && !r.Consumed);

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
            PublicKey = locksmithService.ExportPublicKey(request.PublicKey),
            Version = Guid.NewGuid()
        };

        await db.Users.AddAsync(newUser);

        registrationRequest.Consumed = true;
        registrationRequest.ConsumedAt = DateTime.UtcNow;
        registrationRequest.ConsumedByIp = ip.ToString();
        
        await db.SaveChangesAsync();
    }
}