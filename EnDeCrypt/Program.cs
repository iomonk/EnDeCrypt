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
            EncryptDecryptSelection(userData);
            EncryptOrDecryptData(userData);
        }

        private static void EncryptDecryptSelection(UserData userData)
        {
            Console.WriteLine("Do you want to Encrypt or Decrypt?");
            Console.WriteLine("Encrypt: 1");
            Console.WriteLine("Decrypt: 2");
            
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    userData.Input = "enc";
                    break;
                case "2":
                    userData.Input = "dec";
                    break;
                default:
                    userData.Input = "bad";
                    userData.ErrorMessage = "1 or 2 was not entered.";
                    break;
            }
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
                    Console.Write("Set an 16 character initialization vector (IV) - REMEMBER THIS: ");
                    userData.Iv = Encoding.ASCII.GetBytes(Console.ReadLine() ?? string.Empty);
                    CheckEnteredKeyAndIv(userData);
                    Console.WriteLine("Enter the data you want to encrypt.");
                    userData.DataToEncrypt = Console.ReadLine();
                    var enc = EncryptService.EncryptStringToBytes(userData.DataToEncrypt, userData.Key, userData.Iv);
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
                    Console.Write("Enter your 16 character initialization vector (IV): ");
                    userData.Iv = SetUserDataService.SetUserData();
                    Console.WriteLine();
                    CheckEnteredKeyAndIv(userData);
                    Console.Write("Enter the data you want to decrypt: ");
                    userData.DataToDecrypt = Console.ReadLine();
                    var dec = DecryptService.DecryptStringFromBytes(Convert.FromBase64String(userData.DataToDecrypt ?? string.Empty), userData.Key, userData.Iv);
                    Console.WriteLine("Your decrypted info is: ");
                    Console.WriteLine(dec);
                    break;
                }
                default:
                    ExitApp(userData);
                    break;
            }
        }

        private static void CheckEnteredKeyAndIv(UserData userData)
        {
            if (userData.Key.Length == 32 && userData.Iv.Length == 16) return;
            userData.ErrorMessage = $"Key requires 32 characters. You entered {userData.Key.Length}\nIV requires 16 characters. You entered {userData.Iv.Length}";
            ExitApp(userData);
        }

        private static void ExitApp(UserData userData)
        {
            Console.WriteLine(userData.ErrorMessage);
            Console.WriteLine("Press Enter to exit application.");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}