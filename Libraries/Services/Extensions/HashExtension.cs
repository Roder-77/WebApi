using Services.Helpers;
using System.Text;

namespace Services.Extensions
{
    public static class HashExtension
    {
        public static string ToMD5(this string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return HashHelper.Generate("MD5", bytes);
        }
    }
}
