using System.Threading.Tasks;
using Flustri.Core;
using Flustri.Core.Commands;
using Flustri.Core.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

namespace Flustri.Test;

[TestClass]
public sealed class CreateRoomCommandTests
{
    [TestMethod]
    public async Task TestCreateRoom()
    {
        // Use in-memory database for testing
        var options = new DbContextOptionsBuilder<FlustriDbContext>()
            .UseInMemoryDatabase(databaseName: "TestCreateRoom")
            .Options;

        using var db = new FlustriDbContext(options);
        
        // Ensure database is created
        await db.Database.EnsureCreatedAsync();

        var req = new CreateRoomCommandRequest("test-room");

        await CreateRoomCommand.ConsumeAsync(req, db);

        var room = await db.Rooms.FirstOrDefaultAsync(r => r.RoomId == "test-room");

        Assert.IsNotNull(room);
        Assert.AreEqual("test-room", room.RoomId);
        Assert.AreNotEqual(Guid.Empty, room.Version);
    }
}
