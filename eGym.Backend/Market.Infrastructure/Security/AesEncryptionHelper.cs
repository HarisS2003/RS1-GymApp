using System.Security.Cryptography;
using System.Text;

namespace Market.Infrastructure.Security;

/// <summary>AES-256-CBC with random IV prepended (Base64 payload).</summary>
public sealed class AesEncryptionHelper
{
    private readonly byte[] _key;

    public AesEncryptionHelper(string base64Key)
    {
        if (string.IsNullOrWhiteSpace(base64Key))
        {
            throw new ArgumentException("Encryption key is required.", nameof(base64Key));
        }

        _key = Convert.FromBase64String(base64Key);
        if (_key.Length != 32)
        {
            throw new ArgumentException("AES-256 requires a 32-byte key (Base64-encoded).", nameof(base64Key));
        }
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return plainText;
        }

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var payload = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, payload, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, payload, aes.IV.Length, cipherBytes.Length);

        return Convert.ToBase64String(payload);
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
        {
            return cipherText;
        }

        var payload = Convert.FromBase64String(cipherText);
        if (payload.Length <= 16)
        {
            throw new CryptographicException("Encrypted payload is too short.");
        }

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        var iv = new byte[16];
        var cipherBytes = new byte[payload.Length - iv.Length];
        Buffer.BlockCopy(payload, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(payload, iv.Length, cipherBytes, 0, cipherBytes.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.UTF8.GetString(plainBytes);
    }
}
