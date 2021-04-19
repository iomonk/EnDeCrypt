using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using EnDeCrypt.Services;

namespace EnDeCrypt
{
    public static class Program
    {
        public static void Main()
        {
            var userData = new Models.UserData();
            SetUserInput(userData);
            EncryptOrDecryptData(userData);
        }

        private static void SetUserInput(Models.UserData userData)
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
            Console.WriteLine("1 or 2 was not entered. Press Enter to exit application.");
            Console.ReadLine();
            Environment.Exit(0);
        }

        private static void EncryptOrDecryptData(Models.UserData userData)
        {
            switch (userData.Input)
            {
                case "enc":
                {
                    Console.WriteLine("You have chosen to Encrypt Data.");

                    Console.Write("Set a 32 character Key - REMEMBER THIS: ");
                    userData.Key = Encoding.ASCII.GetBytes(Console.ReadLine() ?? string.Empty);

                    CheckKey(userData);

                    Console.Write("Set an 16 character initialization vector (IV) - REMEMBER THIS: ");
                    userData.Iv = Encoding.ASCII.GetBytes(Console.ReadLine() ?? string.Empty);

                    CheckIv(userData);

                    Console.WriteLine("Enter the data you want to encrypt.");
                    userData.DataToEncrypt = Console.ReadLine();

                    var enc = EncryptDecryptService.EncryptStringToBytes(userData.DataToEncrypt, userData.Key, userData.Iv);
                    Console.WriteLine("Your encrypted string is: ");
                    Console.WriteLine(Convert.ToBase64String(enc));
                    break;
                }
                case "dec":
                {
                    Console.WriteLine("You have chosen to Decrypt Data.");

                    Console.Write("Enter your 32 character Key: ");
                    userData.Key = SetUserData();
                    Console.WriteLine();

                    CheckKey(userData);

                    Console.Write("Enter your 16 character initialization vector (IV): ");
                    userData.Iv = SetUserData();
                    Console.WriteLine();

                    CheckIv(userData);

                    Console.Write("Enter the data you want to decrypt: ");
                    userData.DataToDecrypt = Console.ReadLine();
                    var dataToDecrypt = Convert.FromBase64String(userData.DataToDecrypt ?? string.Empty);

                    var dec = EncryptDecryptService.DecryptStringFromBytes(dataToDecrypt, userData.Key, userData.Iv);
                    Console.WriteLine("Your decrypted info is: ");
                    Console.WriteLine(dec);
                    break;
                }
            }
        }
        
        private static void CheckKey(Models.UserData userData)
        {
            if (userData.Key.Length == 32) return;
            Console.WriteLine("32 characters not entered. Press Enter to exit application.");
            Console.ReadLine();
            Environment.Exit(0);
        }

        private static void CheckIv(Models.UserData userData)
        {
            if (userData.Iv.Length == 16) return;
            Console.WriteLine("16 characters not entered. Press Enter to exit application.");
            Console.ReadLine();
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
    }
}