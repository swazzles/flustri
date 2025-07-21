
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using NSec.Cryptography;

namespace Flustri.Core;

public enum FlustriKeyDerivationAlgorithm
{
    [Description("HKDFSHA256")]
    HkdfSha256 = 0,

    [Description("HKDFSHA512")]
    HkdfSha512 = 1
}

public record LocksmithOptions(
    FlustriKeyDerivationAlgorithm KeyDerivationAlgorithm,
    int SaltSize,
    int MasterPasswordLength
);

public interface ILocksmith
{
    byte[] DeriveKeyFromPassword(string password, long numberOfPasses);
    string GenerateMasterPassword();
}

public class Locksmith : ILocksmith
{
    private LocksmithOptions _options;

    public Locksmith(LocksmithOptions options)
    {
        _options = options;
    }

    public string GenerateMasterPassword()
    {
        return RandomNumberGenerator.GetHexString(_options.MasterPasswordLength).ToLower();
    }

    public byte[] DeriveKeyFromPassword(string password, long numberOfPasses)
    {
        var argon = Argon2id.Argon2id(new Argon2Parameters()
        {
            DegreeOfParallelism = 1,
            MemorySize = 250000, //TODO: Dynamic memory size depending on available memory
            NumberOfPasses = numberOfPasses
        });

        KeyDerivationAlgorithm keyDerivationAlgorithm = _options.KeyDerivationAlgorithm switch
        {
            FlustriKeyDerivationAlgorithm.HkdfSha256 => KeyDerivationAlgorithm.HkdfSha256,
            FlustriKeyDerivationAlgorithm.HkdfSha512 => KeyDerivationAlgorithm.HkdfSha512,
            _ => KeyDerivationAlgorithm.HkdfSha256
        };

        var salt = RandomNumberGenerator.GetBytes(_options.SaltSize);
        var key = argon.DeriveKey(password, salt, keyDerivationAlgorithm);
        var privateKey = key.Export(KeyBlobFormat.PkixPrivateKey);
        return privateKey;
    }
}