using System.Security.Cryptography;
using System.Text;

namespace Server.Utility
{
    internal static class PasswordUtility
    {
        public static string GenerateHash(string basePassword,string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var rng = new Random();
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes($"{salt}{basePassword}"));
                return Encoding.UTF8.GetString(hash);
            }

        }
    }
}
