using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace EnDeCrypt.Services
{
    public static class SetUserDataService
    {
        public static byte[] SetUserData()
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