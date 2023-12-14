using System.ComponentModel;

namespace Common.Extensions
{
    public static class ConverterExtension
    {
        public static T? Parse<T>(this object? value)
        {
            if (value is null) 
                return default;

            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T?)converter.ConvertFrom(value);
        }
    }
}
