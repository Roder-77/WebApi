using AutoMapper;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;
using MiniExcelLibs.OpenXml;
using Models.DataModels;
using Services.Extensions;
using Services.Repositories;
using System.Runtime.CompilerServices;

namespace Services
{
    public class BaseService
    {
        protected readonly HttpContext _httpContext;

        protected readonly ILogger<BaseService> _logger;

        protected readonly IMapper _mapper;

        public BaseService(IServiceProvider serviceProvider)
        {
            _httpContext = serviceProvider.SetDefaultHttpContext().HttpContext!;
            _logger = serviceProvider.GetRequiredService<ILogger<BaseService>>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
        }

        protected string GetCurrentMethod([CallerMemberName] string callerName = "") => callerName;

        /// <summary>
        /// Get file stream result
        /// </summary>
        /// <param name="memoryStream">memory stream</param>
        /// <param name="fileName">file name</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">不支援的媒體類型</exception>
        protected FileStreamResult GetFileStreamResult(MemoryStream memoryStream, string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
                throw new NotImplementedException("不支援的媒體類型");

            return new FileStreamResult(memoryStream, contentType) { FileDownloadName = fileName };
        }

        /// <summary>
        /// Get file content result
        /// </summary>
        /// <param name="bytes">byte array</param>
        /// <param name="fileName">file name</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">不支援的媒體類型</exception>
        protected FileContentResult GetFileContentResult(byte[] bytes, string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
                throw new NotImplementedException("不支援的媒體類型");

            return new FileContentResult(bytes, contentType) { FileDownloadName = fileName };
        }

        /// <summary>
        /// get mini excel memory stream
        /// </summary>
        /// <param name="worksheet">work sheet</param>
        /// <returns></returns>
        protected async Task<MemoryStream> GetMiniExcelMemoryStream(object worksheet)
        {
            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(worksheet, configuration: new OpenXmlConfiguration() { AutoFilter = false });
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        /// <summary>
        /// Get mini excel file stream result
        /// </summary>
        /// <param name="workSheet">work sheet</param>
        /// <param name="fileName">file name</param>
        /// <returns></returns>
        protected async Task<FileStreamResult> GetMiniExcelFileStreamResult(object workSheet, string fileName)
            => GetFileStreamResult(await GetMiniExcelMemoryStream(workSheet), fileName);

        /// <summary>
        /// Compress to zip file
        /// </summary>
        /// <param name="items">memory stream with file name list</param>
        /// <param name="compressionLevel">1 ~ 9</param>
        /// <returns></returns>
        protected async Task<byte[]> CompressToZipFile(IEnumerable<(string fileName, MemoryStream memoryStream)> items, int compressionLevel = 9)
        {
            if (!items.Any())
                throw new ArgumentException("items is empty");

            var memoryStream = new MemoryStream();
            using var zipStream = new ZipOutputStream(memoryStream);

            zipStream.SetLevel(compressionLevel);

            foreach (var item in items)
            {
                var entry = new ZipEntry(item.fileName);
                entry.DateTime = DateTime.Now;
                zipStream.PutNextEntry(entry);

                await item.memoryStream.CopyToAsync(zipStream);
            }

            memoryStream.Position = 0;
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Get zip file content result
        /// </summary>
        /// <param name="items">memory stream with file name list</param>
        /// <param name="fileName">zip file name</param>
        /// <returns></returns>
        protected async Task<FileContentResult> GetZipFileContentResult(IEnumerable<(string fileName, MemoryStream memoryStream)> items, string fileName)
            => GetFileContentResult(await CompressToZipFile(items), fileName);
    }

    public class BaseService<TEntity> : BaseService
        where TEntity : BaseDataModel
    {
        protected readonly IGenericRepository<TEntity> _repository;

        public BaseService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _repository = serviceProvider.GetRequiredService<IGenericRepository<TEntity>>();
        }
    }
}
