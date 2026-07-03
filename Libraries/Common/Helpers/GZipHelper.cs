using System.IO.Compression;
using System.Text;

namespace Common.Helpers
{
    public static class GZipHelper
    {
        /// <summary>
        /// 壓縮
        /// </summary>
        /// <param name="value">要壓縮的字串</param>
        /// <returns>壓縮後的位元組陣列</returns>
        public static async Task<byte[]> Zip(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            using var inputStream = new MemoryStream(bytes);
            using var outputStream = new MemoryStream();
            using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                await inputStream.CopyToAsync(gzipStream);
            }

            return outputStream.ToArray();
        }

        /// <summary>
        /// 解壓縮
        /// </summary>
        /// <param name="bytes">要解壓縮的位元組陣列</param>
        /// <returns>解壓縮後的字串</returns>
        public static async Task<string> Unzip(byte[]? bytes)
        {
            if (bytes == null)
                return "";

            using var inputStream = new MemoryStream(bytes);
            using var outputStream = new MemoryStream();
            using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);

            await gzipStream.CopyToAsync(outputStream);

            return Encoding.UTF8.GetString(outputStream.ToArray());
        }
    }
}