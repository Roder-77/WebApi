using Common.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Models.Exceptions;

namespace Services
{
    public class CommonService : BaseService
    {
        private readonly IWebHostEnvironment _env;

        public CommonService(
            IWebHostEnvironment env,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _env = env;
        }

        /// <summary>
        /// 上傳檔案
        /// </summary>
        /// <param name="file">檔案</param>
        /// <returns></returns>
        /// <exception cref="ForbiddenException"></exception>
        public async Task<(string fileName, string name, string url)> UploadFile(IFormFile file)
        {
            if (file.Length <= 0)
                throw new ForbiddenException($"{nameof(UploadFile)}, name: {file.Name} file size is less than or equal to 0");

            var defaultPath = "uploads";
            var fileExtension = Path.GetExtension(file.FileName);
            var folderPath = fileExtension switch
            {
                ".jpg" or ".jpeg" or ".png" => Path.Combine(defaultPath, "images"),
                ".pdf" => Path.Combine(defaultPath, "pdf"),
                ".mp4" => Path.Combine(defaultPath, "videos"),
                _ => throw new NotImplementedException("不支援上傳此副檔名"),
            };

            var path = Path.Combine(_env.WebRootPath, folderPath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var hash = HashHelper.Generate("SHA1", memoryStream.ToArray());
            var fileName = $"{hash}{fileExtension}";
            var url = $"/{folderPath.Replace("\\", "/")}/{fileName}";

            using var fileStream = File.Create(Path.Combine(path, fileName));
            await file.CopyToAsync(fileStream);

            return (fileName, file.FileName, url);
        }
    }
}
