using System.Security.Cryptography;

namespace Flustri.Core;

public record SigningContextOptions(
    char[] KeyPem,
    HashAlgorithmName HashAlgorithmName
);

public record SignedData(
    byte[] Hash,
    byte[] Signature
);

public class SigningContext
{
    private SigningContextOptions _options;

    public SigningContext(SigningContextOptions options)
    {
        _options = options;
    }

    public byte[] Sign(byte[] data)
    {
        using var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(_options.KeyPem);
        return ecdsa.SignData(data, _options.HashAlgorithmName);        
    }

    public bool Verify(byte[] data, byte[] hash)
    {
        var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(_options.KeyPem);
        return ecdsa.VerifyData(data, hash, _options.HashAlgorithmName);
    }
}