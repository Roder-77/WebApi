namespace Common.Extensions
{
    public static class ListExtension
    {
        /// <summary>
        /// 列表批量
        /// </summary>
        /// <typeparam name="T">class</typeparam>
        /// <param name="items">列表資料</param>
        /// <param name="num">每一次批量筆數</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items, int num = 100) where T : class
        {
            for (int i = 0; i < items.Count(); i += num)
                yield return items.Skip(i).Take(num);
        }

        /// <summary>
        /// 列表是否有值
        /// </summary>
        /// <typeparam name="T">struct</typeparam>
        /// <param name="items">列表資料</param>
        /// <returns></returns>
        public static bool HasValue<T>(this IEnumerable<T>? items) where T : struct
            => items is not null && items.Any();

        /// <summary>
        /// 列表是否有值
        /// </summary>
        /// <param name="items">列表資料</param>
        /// <returns></returns>
        public static bool HasValue(this IEnumerable<object>? items)
            => items is not null && items.Any();
    }
}
