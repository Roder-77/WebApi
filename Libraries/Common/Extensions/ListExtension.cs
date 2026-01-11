using Microsoft.Extensions.Primitives;

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

        /// <summary>
        /// 加入或累加資料
        /// </summary>
        /// <typeparam name="T">not null</typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="count"></param>
        public static void AddOrAccumulate<T>(this Dictionary<T, int> dictionary, T key, int count = 1) where T : notnull
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] += count;
            else
                dictionary[key] = count;
        }

        /// <summary>
        /// List 補長度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">來源列表</param>
        /// <param name="count">要補的數量</param>
        /// <returns></returns>
        public static IEnumerable<T> PadList<T>(this IEnumerable<T> source, int count) where T : class
            => source.Concat(Enumerable.Repeat(Activator.CreateInstance<T>(), count));

        /// <summary>
        /// To StringValues
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static StringValues ToStringValues<T>(this IEnumerable<T> source) where T : class
            => new([.. source.Select(x => x?.ToString() ?? "")]);
    }
}
