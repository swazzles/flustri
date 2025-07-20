namespace Flustri.Core;

public class Room
{
    public required string RoomId { get; set; }
    public required List<User> Users { get; set; }

    public async Task PostMessage(string message)
    {
        
    }
}