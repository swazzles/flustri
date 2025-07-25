
using Flustri.Core;
using Flustri.Core.Commands;
using Microsoft.EntityFrameworkCore;

namespace Flustri.Api;

public class FlustriStartup : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return async builder =>
        {
            using (var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {                
                var dataDir = DataHelper.GetServerDataDirectory();
                if (!Directory.Exists(dataDir))
                    Directory.CreateDirectory(dataDir);

                var db = serviceScope.ServiceProvider.GetRequiredService<FlustriDbContext>();
                await db.Database.EnsureCreatedAsync();

                var locksmith = serviceScope.ServiceProvider.GetRequiredService<ILocksmithService>();
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<FlustriStartup>>();

                await InitialRegistrationCommand.ConsumeAsync(db, logger);
            }


            next(builder);
        };
    }
}