using System.Security.Cryptography;

namespace Flustri.Core.Services;

public interface IServerService
{
    byte[]? ServerMagic { get; }

    Task Setup();
}

public class ServerService : IServerService
{
    public byte[]? ServerMagic { get; private set; }

    public async Task Setup()
    {
        var setupLockLocation = Path.Join(DataHelper.GetServerDataDirectory(), "server.magic");
        if (File.Exists(setupLockLocation))
            return;

        ServerMagic = RandomNumberGenerator.GetBytes(256);
        await File.WriteAllBytesAsync(setupLockLocation, ServerMagic);
    }
}