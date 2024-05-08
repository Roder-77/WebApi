using Common.Enums;

namespace Common.Extensions
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 轉成 Timestamp
        /// </summary>
        /// <param name="source">原日期</param>
        /// <param name="timeUnit">轉換單位</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static long ToTimestamp(this DateTime source, TimeUnit timeUnit = TimeUnit.Milliseconds)
            => timeUnit switch
            {
                TimeUnit.Seconds => ((DateTimeOffset)source).ToUnixTimeSeconds(),
                TimeUnit.Milliseconds => ((DateTimeOffset)source).ToUnixTimeMilliseconds(),
                _ => throw new NotImplementedException("不支援的轉換單位"),
            };

        /// <summary>
        /// 轉成日期
        /// </summary>
        /// <param name="source">原時間戳</param>
        /// <param name="timeUnit">轉換單位</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static DateTime ToDateTime(this long source, TimeUnit timeUnit = TimeUnit.Milliseconds)
            => timeUnit switch
            {
                TimeUnit.Seconds => DateTimeOffset.FromUnixTimeSeconds(source).DateTime.ToLocalTime(),
                TimeUnit.Milliseconds => DateTimeOffset.FromUnixTimeMilliseconds(source).DateTime.ToLocalTime(),
                _ => throw new NotImplementedException("不支援的轉換單位"),
            };

        /// <summary>
        /// 轉成日期字串
        /// </summary>
        /// <param name="source">原時間戳</param>
        /// <param name="format">格式</param>
        /// <param name="timeUnit">轉換單位</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static string ToDateTimeString(this long source, string format, TimeUnit timeUnit = TimeUnit.Milliseconds)
            => source.ToDateTime(timeUnit).ToString(format);

        /// <summary>
        /// 轉成日期字串
        /// </summary>
        /// <param name="source">原時間戳</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static string ToDateTimeString(this long source)
            => source.ToDateTimeString("yyyy-MM-dd hh:mm:ss");
    }
}
