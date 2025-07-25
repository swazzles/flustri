
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using NSec.Cryptography;

namespace Flustri.Core;

public interface ILocksmithService
{
    byte[] DeriveKeyFromPassword(string password, long numberOfPasses);
    string GenerateMasterPassword();
}

public class LocksmithService : ILocksmithService
{

    public const int MasterPasswordLength = 256;
    public const int SaltSize = 32;

    public string GenerateMasterPassword()
    {
        return RandomNumberGenerator.GetHexString(MasterPasswordLength).ToLower();
    }

    public byte[] DeriveKeyFromPassword(string password, long numberOfPasses)
    {
        var argon = Argon2id.Argon2id(new Argon2Parameters()
        {
            DegreeOfParallelism = 1,
            MemorySize = 250000, //TODO: Dynamic memory size depending on available memory
            NumberOfPasses = numberOfPasses
        });

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = argon.DeriveKey(password, salt, KeyDerivationAlgorithm.HkdfSha512);
        var privateKey = key.Export(KeyBlobFormat.PkixPrivateKey);
        return privateKey;
    }
}