using Flustri.Core;

namespace Flustri.Cli;

public static class KeyCommand
{
    public static int Handle(string[] argz) => handleOperation(argz[1].ToLower(), argz);

    private static int handleOperation(string operation, string[] argz) => operation switch
    {
        "new" => newKey(),
        _ => throw new InvalidOperationException($"Invalid operation {operation} for command {argz[0]}")
    };

    private static int newKey()
    {
        var locksmith = new Locksmith(new LocksmithOptions(FlustriKeyDerivationAlgorithm.HkdfSha256, 32));
        var pass = locksmith.GenerateMasterPassword(512);
        Console.WriteLine(pass);        
        return 0;
    }
}