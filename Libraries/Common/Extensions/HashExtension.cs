using Common.Helpers;
using Konscious.Security.Cryptography;
using System.Text;

namespace Common.Extensions
{
    public static class HashExtension
    {
        /// <summary>
        /// To hash by hash algorithm
        /// Algorithm support: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.cryptoconfig?view=net-6.0
        /// </summary>
        /// <param name="source">原值</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <param name="salt">鹽</param>
        /// <returns></returns>
        public static string ToHash(this string source, string hashAlgorithm, string salt = "")
        {
            var bytes = Encoding.UTF8.GetBytes(source.Insert(source.Length / 2, salt));
            return HashHelper.Generate(hashAlgorithm, bytes);
        }

        /// <summary>
        /// To MD5
        /// </summary>
        /// <param name="source">原值</param>
        /// <param name="salt">鹽</param>
        /// <returns></returns>
        public static string ToMD5(this string source, string salt = "")
            => source.ToHash("MD5", salt);

        /// <summary>
        /// To SHA512
        /// </summary>
        /// <param name="source">原值</param>
        /// <param name="salt">鹽</param>
        /// <returns></returns>
        public static string ToSHA512(this string source, string salt = "")
            => source.ToHash("SHA512", salt);

        /// <summary>
        /// To Argon2id
        /// </summary>
        /// <param name="source">原值</param>
        /// <param name="salt">鹽</param>
        /// <returns></returns>
        public static string ToArgon2id(this string source, string salt = "")
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(source));
            argon2.Salt = Encoding.UTF8.GetBytes(salt);
            argon2.DegreeOfParallelism = 4;
            argon2.Iterations = 8;
            argon2.MemorySize = 1024 * 32;

            return Convert.ToBase64String(argon2.GetBytes(32));
        }
    }
}
