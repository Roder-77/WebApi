namespace Common.Extensions
{
    public static class NumericExtension
    {
        /// <summary>
        /// 轉成千分位 (含小數位數四捨五入)
        /// </summary>
        /// <param name="source">原數值</param>
        /// <param name="decimalPointPlace">小數位數</param>
        /// <returns></returns>
        /// <exception cref="FormatException">小數位數僅支援正整數</exception>
        public static string ToThousands(this decimal source, int decimalPointPlace = 3)
        {
            if (decimalPointPlace < 0)
                throw new FormatException("小數位數僅支援正整數");

            return string.Format($@"{{0:N{decimalPointPlace}}}", source);
        }
    }
}
