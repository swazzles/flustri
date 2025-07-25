using System.Diagnostics;
using Flustri.Core.Models;
using Microsoft.Extensions.Logging;

namespace Flustri.Core.Commands;

public static class InitialRegistrationCommand
{
    public static async Task ConsumeAsync(FlustriDbContext db, ILogger logger)
    {
        var setupLockLocation = Path.Join(DataHelper.GetServerDataDirectory(), "setup.lock");
        if (File.Exists(setupLockLocation))
            return;

        var registrationRequest = new RegistrationRequest()
        {
            RegistrationRequestId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            InitialRole = "admin"
        };

        await db.RegistrationRequests.AddAsync(registrationRequest);
        await db.SaveChangesAsync();

        logger.LogInformation($"Initial setup registration request ID: {registrationRequest.RegistrationRequestId}");

        await File.WriteAllTextAsync(setupLockLocation, "To perform initial server setup again please delete this file.");
    }
}