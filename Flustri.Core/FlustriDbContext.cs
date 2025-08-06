using Flustri.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Flustri.Core;

public class FlustriDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RegistrationRequest> RegistrationRequests { get; set; }

    private static string GetDBPath => Path.Join(DataHelper.GetServerDataDirectory(), "flustri.db");

    public FlustriDbContext()
    {
    }

    public FlustriDbContext(DbContextOptions<FlustriDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            options.UseSqlite($"Data Source={GetDBPath}");
        }
    }
}