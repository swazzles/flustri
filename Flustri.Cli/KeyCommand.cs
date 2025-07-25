using System.Threading.Tasks;
using Flustri.Core;

namespace Flustri.Cli;

public static class KeyCommand
{
    public static async Task<int> Handle(string[] argz) => await handleOperation(argz[1].ToLower(), argz);

    private static async Task<int> handleOperation(string operation, string[] argz) => operation switch
    {
        "new" => newKey(),
        "vault" => await vaultKey(argz[2], argz[3]),
        _ => throw new InvalidOperationException($"Invalid operation {operation} for command {argz[0]}")
    };

    private static int newKey()
    {
        var locksmith = new LocksmithService();
        var pass = locksmith.GenerateMasterPassword();
        Console.WriteLine(pass);
        return 0;
    }

    private static async Task<int> vaultKey(string key, string name)
    {
        await File.WriteAllTextAsync(Path.Join(DataHelper.GetClientDataDirectory(), "keys", name), key);
        return 0;
    }
}