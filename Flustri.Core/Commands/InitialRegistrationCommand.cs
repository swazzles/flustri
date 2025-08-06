using System.Diagnostics;
using System.Security.Cryptography;
using Flustri.Core.Models;
using Microsoft.Extensions.Logging;

namespace Flustri.Core.Commands;

public static class InitialRegistrationCommand
{
    public static async Task ConsumeAsync(FlustriDbContext db, ILogger logger)
    {
        if (db.RegistrationRequests.Any(r => r.IsFirstRegistration))
            return;

        var registrationRequest = new RegistrationRequest()
        {
            RegistrationRequestId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            InitialRole = "admin",
            IsFirstRegistration = true
        };

        await db.RegistrationRequests.AddAsync(registrationRequest);
        await db.SaveChangesAsync();

        logger.LogInformation($"Initial setup registration request ID: {registrationRequest.RegistrationRequestId}");
    }
}