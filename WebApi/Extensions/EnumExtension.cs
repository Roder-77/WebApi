using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WebApi.Extensions
{
    public static class EnumExtension
    {
        /// <summary>
        /// Get Enum Name
        /// </summary>
        /// <param name="value">Enum</param>
        /// <param name="isDescription">是否為描述</param>
        /// <returns>Enum Name</returns>
        public static string GetName(this Enum value, bool isDescription = false)
        {
            if (isDescription)
            {
                var descriptionAttribute = GetCustomAttribute<DescriptionAttribute>(value);
                return string.IsNullOrWhiteSpace(descriptionAttribute?.Description) ? value.ToString() : descriptionAttribute.Description;
            }

            var displayAttribute = GetCustomAttribute<DisplayAttribute>(value);
            return string.IsNullOrWhiteSpace(displayAttribute?.Name) ? value.ToString() : displayAttribute.Name;
        }

        private static TAttribute? GetCustomAttribute<TAttribute>(Enum value) where TAttribute : Attribute
        {
            return value.GetType().GetField(value.ToString())?.GetCustomAttribute<TAttribute>(false);
        }
    }
}
