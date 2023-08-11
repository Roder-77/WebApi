using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Common.Enums;

namespace Common.Extensions
{
    public static class EnumExtension
    {
        /// <summary>
        /// Get Enum Name
        /// </summary>
        /// <param name="enumValue">Enum</param>
        /// <param name="type">Attribute type</param>
        /// <returns>Enum Name</returns>
        public static string GetAttributeContent<TEnum>(this TEnum enumValue, AttributeType type = AttributeType.Display) where TEnum : struct
        {
            if (type == AttributeType.Description)
            {
                var descriptionAttribute = GetCustomAttribute<DescriptionAttribute>(enumValue);
                return GetValueOrDefault(descriptionAttribute?.Description);
            }

            var displayAttribute = GetCustomAttribute<DisplayAttribute>(enumValue);
            return GetValueOrDefault(displayAttribute?.Name);

            TAttribute? GetCustomAttribute<TAttribute>(TEnum value) where TAttribute : Attribute
                => value.GetType().GetField(value.ToString()!)?.GetCustomAttribute<TAttribute>(false);

            string GetValueOrDefault(string? value) => string.IsNullOrWhiteSpace(value) ? enumValue.ToString()! : value;
        }
    }
}
