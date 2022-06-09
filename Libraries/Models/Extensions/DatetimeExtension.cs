using Models.Enums;

namespace Models.Extensions
{
    public static class DateTimeExtension
    {
        public static long ToTimestamp(this DateTime datetime, TimeUnit timeUnit = TimeUnit.Milliseconds)
        {
            return timeUnit switch
            {
                TimeUnit.Seconds => ((DateTimeOffset)datetime).ToUnixTimeSeconds(),
                TimeUnit.Milliseconds => ((DateTimeOffset)datetime).ToUnixTimeMilliseconds(),
                _ => throw new NotImplementedException(),
            };
        }

        public static DateTime ToDateTime(this long timestamp, TimeUnit timeUnit = TimeUnit.Milliseconds)
        {
            return timeUnit switch
            {
                TimeUnit.Seconds => DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime.ToLocalTime(),
                TimeUnit.Milliseconds => DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime.ToLocalTime(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
