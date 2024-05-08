using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Common.Enums;

namespace Common.Extensions
{
    public static class EnumExtension
    {
        /// <summary>
        /// Get enum display name
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <returns></returns>
        public static string GetDisplayName<TEnum>(this TEnum enumValue) where TEnum : struct
            => enumValue.GetAttributeContent();

        /// <summary>
        /// Get enum display name
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <returns></returns>
        public static string? GetDisplayName<TEnum>(this TEnum? enumValue) where TEnum : struct
            => enumValue?.GetAttributeContent();

        /// <summary>
        /// Get enum description
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <returns></returns>
        public static string GetDescription<TEnum>(this TEnum enumValue) where TEnum : struct
            => enumValue.GetAttributeContent(AttributeType.Description);

        /// <summary>
        /// Get enum description
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <returns></returns>
        public static string? GetDescription<TEnum>(this TEnum? enumValue) where TEnum : struct
            => enumValue?.GetAttributeContent(AttributeType.Description);

        /// <summary>
        /// Get enum display name
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="num">number</param>
        /// <returns></returns>
        public static string GetDisplayName<TEnum>(this int num) where TEnum : struct
        {
            var value = Enum.Parse<TEnum>(num.ToString(), true);
            return GetAttributeContent(value);
        }

        /// <summary>
        /// Get enum description
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="num">number</param>
        /// <returns></returns>
        public static string GetDescription<TEnum>(this int num) where TEnum : struct
        {
            var value = Enum.Parse<TEnum>(num.ToString(), true);
            return GetAttributeContent(value, AttributeType.Description);
        }

        /// <summary>
        /// Get Enum Name
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum</param>
        /// <param name="type">Attribute type</param>
        /// <returns>Enum Name</returns>
        public static string GetAttributeContent<TEnum>(this TEnum enumValue, AttributeType type = AttributeType.Display) where TEnum : struct
        {
            switch (type)
            {
                case AttributeType.Description:
                    var descriptionAttribute = GetCustomAttribute<DescriptionAttribute>(enumValue);
                    return GetValueOrDefault(descriptionAttribute?.Description);
                case AttributeType.Display:
                    var displayAttribute = GetCustomAttribute<DisplayAttribute>(enumValue);
                    return GetValueOrDefault(displayAttribute?.Name);
                default:
                    throw new NotImplementedException("Unsupport attribute type");
            }

            TAttribute? GetCustomAttribute<TAttribute>(TEnum value) where TAttribute : Attribute
                => value.GetType().GetField(value.ToString()!)?.GetCustomAttribute<TAttribute>(false);

            string GetValueOrDefault(string? value) => string.IsNullOrWhiteSpace(value) ? enumValue.ToString()! : value;
        }
    }
}
