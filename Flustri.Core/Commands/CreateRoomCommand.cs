using Flustri.Core.Models;

namespace Flustri.Core.Commands;

public record CreateRoomCommandRequest (
    string RoomId
);

public static class CreateRoomCommand
{
    public static async Task ConsumeAsync(CreateRoomCommandRequest request, FlustriDbContext db)
    {
        if (db.Rooms.Any(r => r.RoomId == request.RoomId))
            throw new Exception("Room already exists.");

        var newRoom = new Room()
        {
            RoomId = request.RoomId,
            Version = Guid.NewGuid()
        };

        await db.Rooms.AddAsync(newRoom);
        await db.SaveChangesAsync();
    }
}