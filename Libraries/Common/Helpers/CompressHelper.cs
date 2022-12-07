using ICSharpCode.SharpZipLib.Zip;

namespace Common.Helpers
{
    public class CompressHelper
    {
        /// <summary>
        /// 多個檔案壓縮成 Zip 檔
        /// </summary>
        /// <param name="sourcePath">來源路徑</param>
        /// <param name="outputFilePath">輸出檔案路徑</param>
        /// <param name="excludePaths">排除的路徑列表</param>
        /// <param name="compressionLevel">壓縮等級 0 ~ 9</param>
        public static void CompressFilesToZip(string sourcePath, string outputFilePath, IEnumerable<string>? excludePaths = null, int compressionLevel = 9)
        {
            try
            {
                using var outputStream = new ZipOutputStream(File.Create(outputFilePath));
                // 0 - store only to 9 - means best compression
                outputStream.SetLevel(compressionLevel);

                foreach (var filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    if (excludePaths != null && excludePaths.Any(x => filePath.Contains(x)))
                        continue;

                    WriteFile(outputStream, filePath);
                }

                outputStream.Finish();
                outputStream.Close();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 單一檔案壓縮成 Zip 檔
        /// </summary>
        /// <param name="filePath">檔案路徑</param>
        /// <param name="outputFilePath">輸出檔案路徑</param>
        /// <param name="compressionLevel">壓縮等級 0 ~ 9</param>
        public static void CompressFileToZip(string filePath, string outputFilePath, int compressionLevel = 9)
        {
            try
            {
                using var outputStream = new ZipOutputStream(File.Create(outputFilePath));
                // 0 - store only to 9 - means best compression
                outputStream.SetLevel(compressionLevel);

                WriteFile(outputStream, filePath);

                outputStream.Finish();
                outputStream.Close();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 寫入檔案
        /// </summary>
        /// <param name="outputStream">zip output stream</param>
        /// <param name="filePath">檔案路徑</param>
        private static void WriteFile(ZipOutputStream outputStream, string filePath)
        {
            var buffer = new byte[4096];
            var fileName = Path.GetFileName(filePath);
            var entry = new ZipEntry(fileName);
            entry.DateTime = DateTime.Now;
            outputStream.PutNextEntry(entry);

            using (var fs = File.OpenRead(filePath))
            {
                int sourceBytes;

                do
                {
                    sourceBytes = fs.Read(buffer, 0, buffer.Length);
                    outputStream.Write(buffer, 0, sourceBytes);
                } while (sourceBytes > 0);
            }
        }
    }
}
