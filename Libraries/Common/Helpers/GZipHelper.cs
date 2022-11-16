using System.IO.Compression;
using System.Text;

namespace Common.Helpers
{
    public class GZipHelper
    {
        private static void CopyTo(Stream sourceStream, Stream destinationStream)
        {
            var bytes = new byte[4096];
            int cnt;

            while ((cnt = sourceStream.Read(bytes, 0, bytes.Length)) != 0)
                destinationStream.Write(bytes, 0, cnt);
        }

        /// <summary>
        /// 壓縮
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Zip(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            using var inputStream = new MemoryStream(bytes);
            using var outputStream = new MemoryStream();
            using var gzipStream = new GZipStream(outputStream, CompressionMode.Compress);

            CopyTo(inputStream, gzipStream);

            return outputStream.ToArray();
        }

        /// <summary>
        /// 解壓縮
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Unzip(byte[]? bytes)
        {
            if (bytes == null)
                return "";

            using var inputStream = new MemoryStream(bytes);
            using var outputStream = new MemoryStream();
            using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);

            CopyTo(gzipStream, outputStream);

            return Encoding.UTF8.GetString(outputStream.ToArray());
        }
    }
}
