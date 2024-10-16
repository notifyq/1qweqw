using System;
using System.Security.Cryptography;
using System.Text;


namespace team_project.Api
{
    internal class UserInfoCrypt
    {
        public static string EncryptString(string input)
        {
            byte[] data = Encoding.Unicode.GetBytes(input);
            byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        public static string DecryptString(string encryptedData)
        {
            byte[] data = Convert.FromBase64String(encryptedData);
            byte[] decrypted = ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
            return Encoding.Unicode.GetString(decrypted);
        }

    }
}
