using System.Security.Cryptography;

namespace Flustri.Core;

public interface IServer
{
    List<Room> Rooms { get; set; }

    User GetUserById(string userId);
}

public class Server : IServer
{
    public required List<Room> Rooms { get; set; }

    public User GetUserById(string userId)
    {
        return new User()
        {
            UserId = userId,
            KeyPem = Array.Empty<char>()
        };
    }
}