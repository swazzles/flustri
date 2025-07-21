using Flustri.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Flustri.Core;

public class FlustriDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RegistrationRequest> RegistrationRequests { get; set; }

    public required string DbPath { get; set; }

    public FlustriDbContext()
    {
        DbPath = DataHelper.GetDataFilePath("flustri.db");        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}