using NSec.Cryptography;

namespace Flustri.Core.Services;

public record SignedData(
    byte[] Hash,
    byte[] Signature
);

public interface ISigningService
{
    byte[] Sign(Key privateKey, byte[] data);
    bool Verify(PublicKey publicKey, byte[] data, byte[] signature);
}

public class SigningService : ISigningService
{

    public byte[] Sign(Key privateKey, byte[] data)
    {
        return SignatureAlgorithm.Ed25519.Sign(privateKey, data);
    }

    public bool Verify(PublicKey publicKey, byte[] data, byte[] signature)
    {
        return SignatureAlgorithm.Ed25519.Verify(publicKey, data, signature);
    }
}