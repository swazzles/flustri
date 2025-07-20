using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace Flustri.Core;

public interface IServer
{
    Task<List<Room>> GetRooms();

    Task<User?> GetUserByIdAsync(string userId);
}

public class Server : IServer
{
    private FlustriDbContext _db;
    public Server(FlustriDbContext db)
    {
        _db = db;
    }

    public async Task<List<Room>> GetRooms()
    {
        return await _db.Rooms.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }
}