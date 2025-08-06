using System.Security.Cryptography;
using NSec.Cryptography;

namespace Flustri.Core.Services;

public interface ILocksmithService
{
    Key DeriveKeyFromPassword(string password, long numberOfPasses);
    string GenerateMasterPassword();
    byte[] ExportPublicKey(PublicKey publicKey);
    byte[] ExportPrivateKey(Key privateKey);

}


public class LocksmithService : ILocksmithService
{

    public const int MasterPasswordLength = 256;
    public const int SaltSize = 16;

    public string GenerateMasterPassword()
    {
        return RandomNumberGenerator.GetHexString(MasterPasswordLength).ToLower();
    }

    public Key DeriveKeyFromPassword(string password, long numberOfPasses)
    {
        var argon = PasswordBasedKeyDerivationAlgorithm.Argon2id(new Argon2Parameters()
        {
            DegreeOfParallelism = 1,
            MemorySize = 250000, //TODO: Dynamic memory size depending on available memory
            NumberOfPasses = numberOfPasses
        });

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = argon.DeriveKey(password, salt, KeyDerivationAlgorithm.HkdfSha512);

        return key;
    }

    public byte[] ExportPublicKey(PublicKey publicKey)
    {
        return publicKey.Export(KeyBlobFormat.NSecPublicKey);
    }

    public byte[] ExportPrivateKey(Key privateKey)
    {
        return privateKey.Export(KeyBlobFormat.NSecPrivateKey);
    }
}