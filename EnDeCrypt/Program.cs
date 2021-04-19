using System;
using System.Text;
using EnDeCrypt.Models;
using EnDeCrypt.Services;

namespace EnDeCrypt
{
    public static class Program
    {
        public static void Main()
        {
            var userData = new UserData();
            SelectToEncryptOrDecrypt(userData);
            EncryptOrDecryptData(userData);
        }

        private static void SelectToEncryptOrDecrypt(UserData userData)
        {
            Console.WriteLine("Do you want to Encrypt or Decrypt?");
            Console.Write("1: Encrypt 2: Decrypt: ");
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

        private static void EncryptOrDecryptData(UserData userData)
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
                    userData.Key = SetUserDataService.SetUserData();
                    Console.WriteLine();

                    CheckKey(userData);

                    Console.Write("Enter your 16 character initialization vector (IV): ");
                    userData.Iv = SetUserDataService.SetUserData();
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

        private static void CheckKey(UserData userData)
        {
            if (userData.Key.Length == 32) return;
            Console.WriteLine("32 characters not entered. Press Enter to exit application.");
            Console.ReadLine();
            Environment.Exit(0);
        }

        private static void CheckIv(UserData userData)
        {
            if (userData.Iv.Length == 16) return;
            Console.WriteLine("16 characters not entered. Press Enter to exit application.");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}