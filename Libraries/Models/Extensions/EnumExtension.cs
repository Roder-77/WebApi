using Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Models.Extensions
{
    public static class EnumExtension
    {
        /// <summary>
        /// Get Enum Name
        /// </summary>
        /// <param name="value">Enum</param>
        /// <param name="type">Attribute type</param>
        /// <returns>Enum Name</returns>
        public static string GetAttributeContent(this Enum value, AttributeType type = AttributeType.Display)
        {
            if (type == AttributeType.Description)
            {
                var descriptionAttribute = GetCustomAttribute<DescriptionAttribute>(value);
                return string.IsNullOrWhiteSpace(descriptionAttribute?.Description) ? value.ToString() : descriptionAttribute.Description;
            }

            var displayAttribute = GetCustomAttribute<DisplayAttribute>(value);
            return string.IsNullOrWhiteSpace(displayAttribute?.Name) ? value.ToString() : displayAttribute.Name;

            TAttribute? GetCustomAttribute<TAttribute>(Enum value) where TAttribute : Attribute
                => value.GetType().GetField(value.ToString())?.GetCustomAttribute<TAttribute>(false);
        }
    }
}
