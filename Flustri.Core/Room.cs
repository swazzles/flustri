namespace Flustri.Core;

public class Room
{
    public required string Name { get; set; }
    public required List<User> Users { get; set; }

    public void SendMessage(string message)
    {
        

    }
}