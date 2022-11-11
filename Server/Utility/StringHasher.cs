using Konscious.Security.Cryptography;
using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace Server.Utility
{
    internal static class StringHasher
    {
        public static string HashString(string stringToHash,string salt)
        {
            var strBytes = Encoding.ASCII.GetBytes(stringToHash);
            var saltBytes = Encoding.ASCII.GetBytes(salt);
            var argon2 = new Argon2id(strBytes)
            {
                Salt = saltBytes,
                MemorySize = 8192, //in KB -- To make hash cracking more expensive for an attacker, you want to make this value as high as possible
                DegreeOfParallelism = 160, // This should be chosen as high as possible to reduce the threat imposed by parallelized hash cracking
                Iterations = 1, // The execution time correlates linearly with this parameter. It allows you to increase the computational cost required to calculate one hash 
            };
            var bytes = argon2.GetBytes(128);
            return Convert.ToBase64String(bytes);
        }
        public static string HashString(string stringToHash)
        {
            return HashString(stringToHash, "");
        }
    }
}
