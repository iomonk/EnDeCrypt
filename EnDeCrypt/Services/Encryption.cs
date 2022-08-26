using System;
using System.IO;
using System.Security.Cryptography;

namespace EnDeCrypt.Services;

public static class Encryption
{
    public static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
    {
        if (plainText is not {Length: > 0})
            throw new ArgumentNullException(nameof(plainText));
        if (key is not {Length: > 0})
            throw new ArgumentNullException(nameof(key));
        if (iv is not {Length: > 0})
            throw new ArgumentNullException(nameof(iv));

        using var aesAlg = new AesManaged {Key = key, IV = iv};

        var encrypt = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encrypt, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        var encrypted = msEncrypt.ToArray();

        return encrypted;
    }
}