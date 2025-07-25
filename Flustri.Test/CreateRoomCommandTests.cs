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
        var db = new Mock<FlustriDbContext>();
        db.Setup<DbSet<Room>>(x => x.Rooms)
            .ReturnsDbSet(new List<Room>() { });

        var req = new CreateRoomCommandRequest("test-room");

        await CreateRoomCommand.ConsumeAsync(req, db.Object);

        var room = await db.Object.Rooms.FirstOrDefaultAsync(r => r.RoomId == "test-room");

        Assert.IsNotNull(room);
    }
}
