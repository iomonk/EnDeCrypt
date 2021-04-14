using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace EnDeCrypt
{
    internal static class Program
    {
        public static void Main()
        {
            var userData = new UserData();
            SetUserInput(userData);
            EncryptOrDecryptData(userData);
        }

        private static void EncryptOrDecryptData(UserData userData)
        {
            switch (userData.Input)
            {
                case "enc":
                {
                    Console.WriteLine("You have chosen to Encrypt Data.");

                    Console.Write("Set a 32 character Key - REMEMBER THIS: ");
                    userData.Key = Encoding.ASCII.GetBytes(Console.ReadLine() ?? string.Empty);

                    if (userData.Key.Length != 32)
                    {
                        Console.WriteLine("32 characters not entered. Exiting application");
                        Environment.Exit(0);
                    }

                    Console.Write("Set an 16 character initialization vector (IV) - REMEMBER THIS: ");
                    userData.Iv = Encoding.ASCII.GetBytes(Console.ReadLine() ?? string.Empty);

                    if (userData.Iv.Length != 16)
                    {
                        Console.WriteLine("16 characters not entered. Exiting application");
                        Environment.Exit(0);
                    }

                    Console.WriteLine("Enter the data you want to encrypt.");
                    userData.DataToEncrypt = Console.ReadLine();

                    var enc = EncryptStringToBytes_Aes(userData.DataToEncrypt, userData.Key, userData.Iv);
                    Console.WriteLine("Your encrypted string is: ");
                    Console.WriteLine();
                    Console.WriteLine(Convert.ToBase64String(enc));
                    break;
                }
                case "dec":
                {
                    Console.WriteLine("You have chosen to Decrypt Data.");

                    Console.Write("Enter your 32 character Key: ");
                    userData.Key = SetUserData();
                    Console.WriteLine();

                    if (userData.Key.Length != 32)
                    {
                        Console.WriteLine("32 characters not entered. Exiting application");
                        Environment.Exit(0);
                    }

                    Console.Write("Enter your 16 character initialization vector (IV): ");
                    userData.Iv = SetUserData();
                    Console.WriteLine();

                    if (userData.Iv.Length != 16)
                    {
                        Console.WriteLine("16 characters not entered. Exiting application");
                        Environment.Exit(0);
                    }

                    Console.Write("Enter the data you want to decrypt: ");
                    userData.DataToDecrypt = Console.ReadLine();
                    var dataToDecrypt = Convert.FromBase64String(userData.DataToDecrypt ?? string.Empty);

                    var dec = DecryptStringFromBytes_Aes(dataToDecrypt, userData.Key, userData.Iv);
                    Console.WriteLine("Your decrypted info is: ");
                    Console.WriteLine();
                    Console.WriteLine(dec);
                    break;
                }
            }
        }

        private static void SetUserInput(UserData userData)
        {
            Console.Write("Do you want to Encrypt or Decrypt? 1: Encrypt 2: Decrypt: ");
            var input = Console.ReadLine();

            userData.Input = input switch
            {
                "1" => "enc",
                "2" => "dec",
                _ => "bad"
            };

            if (userData.Input != "bad") return;
            Console.WriteLine("1 or 2 was not entered. Exiting application");
            Environment.Exit(0);
        }

        private static byte[] SetUserData()
        {
            using var securePwd = new SecureString();
            ConsoleKeyInfo key;

            Console.Write("Enter Key: ");
            do
            {
                key = Console.ReadKey(true);

                if (key.KeyChar == 8)
                {
                    securePwd.RemoveAt(securePwd.Length - 1);
                    Console.Write("\b \b");
                }

                if ((int) key.Key < 33 || (int) key.Key > 127) continue;
                securePwd.AppendChar(key.KeyChar);
                Console.Write("*");
            } while (key.Key != ConsoleKey.Enter);

            var ud1 = Marshal.SecureStringToGlobalAllocUnicode(securePwd);
            var ud2 = Marshal.PtrToStringUni(ud1);
            return Encoding.ASCII.GetBytes(ud2 ?? string.Empty);
        }

        private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));
            byte[] encrypted;

            // Create an AesManaged object
            // with the specified key and IV.
            using (var aesAlg = new AesManaged())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create an encryptor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            // Declare the string used to hold
            // the decrypted text.
            string plaintext;

            // Create an AesManaged object
            // with the specified key and IV.
            using (var aesAlg = new AesManaged())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decryptor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}