namespace Common.Extensions
{
    public static class StringExtension
    {
        public static IEnumerable<string> SplitAndTrim(this string? source, string separator)
        {
            if (string.IsNullOrWhiteSpace(source))
                return Enumerable.Empty<string>();

            return source.Split(separator).Select(x => x.Trim());
        }
    }
}
