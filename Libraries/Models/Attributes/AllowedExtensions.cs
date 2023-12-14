using Common.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AllowedExtensions : ValidationAttribute
    {
        private readonly List<string>? _extensions;

        public AllowedExtensions(UploadType fileType)
        {
            _extensions = fileType switch
            {
                UploadType.Excel => new() { ".csv", ".xlsx" },
                _ => null,
            };
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (_extensions is null || value is null)
                return ValidationResult.Success;

            if (!IsList(value))
            {
                var file = (value as IFormFile)!;

                if (!CheckFileExtension(file))
                    return new ValidationResult(GetErrorMessage());

                return ValidationResult.Success;
            }

            var files = (value as IEnumerable<IFormFile>)!;
            foreach (var file in files)
                if (!CheckFileExtension(file))
                    return new ValidationResult(GetErrorMessage());

            return ValidationResult.Success;
        }

        private bool IsList(object obj)
        {
            var type = obj.GetType();

            return obj is IEnumerable<IFormFile>
                && type.IsGenericType
                && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        private bool CheckFileExtension(IFormFile file) => _extensions!.Contains(Path.GetExtension(file.FileName).ToLower());

        private string GetErrorMessage() => $"僅支援匯入 {string.Join(", ", _extensions!)} 格式";
    }
}
