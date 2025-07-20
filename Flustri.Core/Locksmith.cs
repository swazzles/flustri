
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using NSec.Cryptography;

namespace Flustri.Core;

public record FlustriKey(
    byte[] PrivateKey,
    byte[] PublicKey
);

public enum FlustriKeyDerivationAlgorithm
{
    [Description("HKDFSHA256")]
    HkdfSha256 = 0,

    [Description("HKDFSHA512")]
    HkdfSha512 = 1
}

public record LocksmithOptions(
    FlustriKeyDerivationAlgorithm KeyDerivationAlgorithm,
    int SaltSize
);

public class Locksmith
{
    private LocksmithOptions _options;

    public Locksmith(LocksmithOptions options)
    {
        _options = options;
    }

    public string GenerateMasterPassword(int length)
    {
        return RandomNumberGenerator.GetHexString(length).ToLower();
    }

    public FlustriKey DeriveKeyFromPassword(string password, long memorySize, long numberOfPasses)
    {
        var argon = Argon2id.Argon2id(new Argon2Parameters()
        {
            DegreeOfParallelism = 1,
            MemorySize = memorySize,
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
        var privateKey = key.Export(KeyBlobFormat.RawPrivateKey);
        var publicKey = key.Export(KeyBlobFormat.RawPublicKey);

        return new FlustriKey(privateKey, publicKey);
    }
}