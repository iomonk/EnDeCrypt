using System;
using System.IO;
using System.Security.Cryptography;

namespace EnDeCrypt.Services;

public static class Decryption
{
    public static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
    {
        if (cipherText is not {Length: > 0})
            throw new ArgumentNullException(nameof(cipherText));
        if (key is not {Length: > 0})
            throw new ArgumentNullException(nameof(key));
        if (iv is not {Length: > 0})
            throw new ArgumentNullException(nameof(iv));

        using var aesAlg = new AesManaged {Key = key, IV = iv};

        var decrypt = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using var msDecrypt = new MemoryStream(cipherText);
        using var csDecrypt = new CryptoStream(msDecrypt, decrypt, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        var plaintext = srDecrypt.ReadToEnd();

        return plaintext;
    }
}