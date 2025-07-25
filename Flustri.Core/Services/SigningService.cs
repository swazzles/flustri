using System.Security.Cryptography;
using NSec.Cryptography;

namespace Flustri.Core;

public record SignedData(
    byte[] Hash,
    byte[] Signature
);

public interface ISigningService
{
    byte[] Sign(byte[] privateKey, byte[] data);
    bool Verify(byte[] publicKey, byte[] data, byte[] signature);
}

public class SigningService : ISigningService
{

    public byte[] Sign(byte[] privateKey, byte[] data)
    {
        var key = Key.Import(SignatureAlgorithm.Ed25519, privateKey, KeyBlobFormat.PkixPrivateKey);
        return SignatureAlgorithm.Ed25519.Sign(key, data);
    }

    public bool Verify(byte[] publicKey, byte[] data, byte[] signature)
    {
        var key = PublicKey.Import(SignatureAlgorithm.Ed25519, publicKey, KeyBlobFormat.PkixPublicKey);
        return SignatureAlgorithm.Ed25519.Verify(key, data, signature);
    }
}