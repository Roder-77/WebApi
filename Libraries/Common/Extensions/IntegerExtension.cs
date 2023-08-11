namespace Common.Extensions
{
    public static class IntegerExtension
    {
        /// <summary>
        /// 是否為奇數
        /// </summary>
        /// <param name="source">原整數</param>
        /// <returns></returns>
        public static bool IsOdd(this int source) => Convert.ToBoolean(source & 1);

        /// <summary>
        /// 向左補字元
        /// </summary>
        /// <param name="source">原整數</param>
        /// <param name="totalWidth">總寬度</param>
        /// <param name="paddingChar">字元</param>
        /// <returns></returns>
        public static string PadLeft(this int source, int totalWidth, char paddingChar) => source.ToString().PadLeft(totalWidth, paddingChar);
    }
}
