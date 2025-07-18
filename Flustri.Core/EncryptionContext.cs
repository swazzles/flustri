using System.Security.Cryptography;

namespace Flustri.Core;

public record EncryptionContextOptions (
    byte[] SharedKey
);

public record EncryptedData(
    byte[] Nonce,
    byte[] Ciphertext,
    byte[] Tag
);

public class EncryptionContext
{
    private EncryptionContextOptions _options;

    public EncryptionContext(EncryptionContextOptions options)
    {
        _options = options;
    }

    public EncryptedData Encrypt(byte[] data)
    {
        using var crypto = new ChaCha20Poly1305(_options.SharedKey);
        var nonce = RandomNumberGenerator.GetBytes(12);
        var tag = RandomNumberGenerator.GetBytes(16);
        var encrypted = Span<byte>.Empty;
        crypto.Encrypt(nonce, data, encrypted, tag);
        return new EncryptedData(nonce, encrypted.ToArray(), tag);
    }

    public byte[] Decrypt(EncryptedData data)
    {
        var decrypted = Span<byte>.Empty;
        using var crypto = new ChaCha20Poly1305(_options.SharedKey);
        crypto.Decrypt(data.Nonce, data.Ciphertext, data.Tag, decrypted);
        return decrypted.ToArray();
    }
}