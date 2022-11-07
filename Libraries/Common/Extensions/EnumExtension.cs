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
        public static string GetAttributeContent(this Enum enumValue, AttributeType type = AttributeType.Display)
        {
            if (type == AttributeType.Description)
            {
                var descriptionAttribute = getCustomAttribute<DescriptionAttribute>(enumValue);
                return getValueOrDefault(descriptionAttribute?.Description);
            }

            var displayAttribute = getCustomAttribute<DisplayAttribute>(enumValue);
            return getValueOrDefault(displayAttribute?.Name);

            TAttribute? getCustomAttribute<TAttribute>(Enum value) where TAttribute : Attribute
                => value.GetType().GetField(value.ToString())?.GetCustomAttribute<TAttribute>(false);

            string getValueOrDefault(string? value) => string.IsNullOrWhiteSpace(value) ? enumValue.ToString() : value;
        }
    }
}
