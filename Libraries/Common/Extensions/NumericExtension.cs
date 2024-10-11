namespace Common.Extensions
{
    public static class NumericExtension
    {
        /// <summary>
        /// 轉成千分位 (含小數位數四捨五入)
        /// </summary>
        /// <param name="source">原數值</param>
        /// <param name="decimals">小數位數</param>
        /// <returns></returns>
        /// <exception cref="FormatException">小數位數僅支援正整數, 不支援 int、decimal 以外型態</exception>
        public static string ToThousands(this object source, int decimals = 3)
        {
            if (decimals < 0)
                throw new FormatException("小數位數僅支援正整數");

            if (source is int iSource)
                return iSource.ToString($"#,##0.{new string('#', decimals)}");
            else if (source is decimal dSource)
                return dSource.ToString($"#,##0.{new string('#', decimals)}");
            else
                throw new FormatException("不支援 int、decimal 以外型態");
        }

        /// <summary>
        /// 四捨五入
        /// </summary>
        /// <param name="source">原數值</param>
        /// <param name="decimals">小數位數</param>
        /// <returns></returns>
        public static decimal RoundAwayFromZero(this decimal source, int decimals = 3)
            => Math.Round(source, decimals, MidpointRounding.AwayFromZero);

        /// <summary>
        /// 四捨五入
        /// </summary>
        /// <param name="source"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static decimal? RoundAwayFromZero(this decimal? source, int decimals = 3)
        {
            if (!source.HasValue)
                return source;

            return RoundAwayFromZero(source.Value, decimals);
        }

        /// <summary>
        /// 是否為整數
        /// </summary>
        /// <param name="source">來源數值</param>
        /// <returns></returns>
        public static bool IsInteger(this decimal source) => source == Math.Floor(source);
    }
}
