using System.IO.Compression;
using System.Text;

namespace Common.Helpers
{
    public class GZipHelper
    {
        /// <summary>
        /// 壓縮
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
        /// <param name="bytes"></param>
        /// <returns></returns>
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
