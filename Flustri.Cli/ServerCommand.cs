using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Flustri.Core;
using Flustri.Core.Commands;
using Flustri.Core.Services;

namespace Flustri.Cli;

public static class ServerCommand
{
    public static async Task<int> Handle(string[] argz) => await handleOperation(argz[1].ToLower(), argz);

    private static async Task<int> handleOperation(string operation, string[] argz) => operation switch
    {
        "register" => await register(argz[2], argz[3], argz[4]),        
        _ => throw new InvalidOperationException($"Invalid operation {operation} for command {argz[0]}")
    };

    private static async Task<int> register(string server, string requestId, string nickname)
    {
        var uriBuilder = new UriBuilder(server);
        uriBuilder.Path = "/register";

        var locksmith = new LocksmithService();
        var masterKey = locksmith.GenerateMasterPassword();
        var keyId = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes($"{server}{nickname}")));

        var keyPath = Path.Join(DataHelper.GetClientDataDirectory(), "keys", keyId);
        new FileInfo(keyPath).Directory?.Create();

        await File.WriteAllTextAsync(keyPath, masterKey);

        var key = locksmith.DeriveKeyFromPassword(masterKey, 1000);
        var signingService = new SigningService();
        var requestIdGuid = Guid.Parse(requestId);
        var sig = signingService.Sign(key, requestIdGuid.ToByteArray());


        var req = new RegisterCommandRequest(requestIdGuid, key.PublicKey, sig, nickname);

        using var httpClient = new HttpClient();
        var res = await httpClient.PostAsJsonAsync(uriBuilder.Uri, req);        
        res.EnsureSuccessStatusCode();        
        return 0;
    }
}