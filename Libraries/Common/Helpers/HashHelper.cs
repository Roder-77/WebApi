using System.Security.Cryptography;

#nullable disable

namespace Common.Helpers
{
    public class HashHelper
    {
        /// <summary>
        /// Generate a data hash
        /// Algorithm support: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.cryptoconfig?view=net-6.0
        /// </summary>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <param name="data">The data for calculating the hash</param>
        /// <param name="trimByteCount">The number of bytes, which will be used in the hash algorithm; leave 0 to use all array</param>
        /// <returns>Data hash</returns>
        public static string Generate(string hashAlgorithm, byte[] data, int trimByteCount = 0)
        {
            if (string.IsNullOrWhiteSpace(hashAlgorithm))
                throw new ArgumentNullException(nameof(hashAlgorithm));

            var algorithm = (HashAlgorithm)CryptoConfig.CreateFromName(hashAlgorithm);

            if (algorithm == null)
                throw new ArgumentException("Unrecognized hash name");

            if (trimByteCount > 0 && data.Length > trimByteCount)
            {
                var newData = new byte[trimByteCount];
                Array.Copy(data, newData, trimByteCount);

                return dataToHashString(newData);
            }

            return dataToHashString(data);

            string dataToHashString(byte[] data) => BitConverter.ToString(algorithm.ComputeHash(data)).Replace("-", string.Empty);
        }
    }
}
