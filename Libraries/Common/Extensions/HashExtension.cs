using System.Text;
using Common.Helpers;

namespace Common.Extensions
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
