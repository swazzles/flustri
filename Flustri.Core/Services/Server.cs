using Microsoft.EntityFrameworkCore;
using Flustri.Core.Models;

namespace Flustri.Core.Services;


public class FlustriServer
{
    private FlustriDbContext _db;
    public FlustriServer(FlustriDbContext db)
    {
        _db = db;
    }

    public async Task UpdateRoom(Room room)
    {
        var existingRoom = await _db.Rooms.FirstOrDefaultAsync(r => r.RoomId == room.RoomId);
        if (existingRoom is null)
            throw new Exception("Room does not exist.");

        if (existingRoom.Version != room.Version)
            throw new Exception("Room version is behind, please fetch the room again before modifying.");

        room.Version = Guid.NewGuid();
            
        await _db.SaveChangesAsync();
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }
}