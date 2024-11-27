using Common.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AllowedExtensions : ValidationAttribute
    {
        private readonly List<string>? _extensions;
        private readonly int? _sizelimit;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="fileType">檔案類型</param>
        public AllowedExtensions(UploadType fileType)
        {
            _extensions = fileType switch
            {
                UploadType.Excel => [".csv", ".xlsx"],
                UploadType.Image => [".png", ".jpg", ".jpeg"],
                UploadType.PDF => [".pdf"],
                UploadType.Video => [".mp4"],
                _ => null,
            };
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="fileType">檔案類型</param>
        /// <param name="sizeLimit">大小限制 MB</param>
        public AllowedExtensions(UploadType fileType, int sizeLimit) : this(fileType)
        {
            _sizelimit = sizeLimit;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (_extensions is null || value is null)
                return ValidationResult.Success;

            if (value is IFormFile file)
                return Valid(file);
            else if (value is IEnumerable<IFormFile> files)
            {
                foreach (var item in files)
                {
                    if (!IsValid(item, out var message))
                        return new ValidationResult(message);
                }

                return ValidationResult.Success;
            }

            return ValidationResult.Success;
        }

        private bool IsValid(IFormFile file, out string? message)
        {
            if (!_extensions!.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                message = $"僅支援匯入 {string.Join(", ", _extensions!)} 格式";
                return false;
            }

            if (_sizelimit is not null && file.Length > _sizelimit * 1024 * 1024)
            {
                message = $"檔案大小超過限制 {_sizelimit} MB";
                return false;
            }

            message = null;
            return true;
        }

        private ValidationResult? Valid(IFormFile? file)
        {
            if (file is null)
                return ValidationResult.Success;

            if (!IsValid(file, out var message))
                return new ValidationResult(message);

            return ValidationResult.Success;
        }
    }
}
