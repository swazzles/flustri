using Flustri.Core.Models;

namespace Flustri.Core.Commands;

public record CreateRoomRequest (
    string RoomId
);

public static class CreateRoomConsumer
{
    public static async Task ConsumeAsync(CreateRoomRequest request, FlustriDbContext db)
    {
        if (db.Rooms.Any(r => r.RoomId == request.RoomId))
            throw new Exception("Room already exists.");

        var newRoom = new Room()
        {
            RoomId = request.RoomId,
            Version = Guid.NewGuid()
        };

        await db.AddAsync(newRoom);
        await db.SaveChangesAsync();
    }
}