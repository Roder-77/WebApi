﻿using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Services.Extensions
{
    public static class PaginationExtension
    {
        public static PaginationList<T> ToPaginationList<T>(this IEnumerable<T> items, int page, int pageSize = 20) where T : class
        {
            var skip = (page - 1) * pageSize;

            return new PaginationList<T>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = items.Count(),
                Items = items.Skip(skip).Take(pageSize).ToList(),
            };
        }

        public static async Task<PaginationList<T>> ToPaginationList<T>(this IQueryable<T> items, int page, int pageSize = 20) where T : class
        {
            var skip = (page - 1) * pageSize;

            return new PaginationList<T>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = await items.CountAsync(),
                Items = await items.Skip(skip).Take(pageSize).ToListAsync(),
            };
        }

        public class PaginationList<T> where T : class
        {
            /// <summary>
            /// 當前頁數
            /// </summary>
            public int Page { get; set; }

            /// <summary>
            /// 每頁筆數
            /// </summary>
            public int PageSize { get; set; }

            /// <summary>
            /// 總數量
            /// </summary>
            public int TotalCount { get; set; }

            /// <summary>
            /// 總頁數
            /// </summary>
            public int TotalPages => (int)Math.Ceiling((decimal)TotalCount / PageSize);

            /// <summary>
            /// 是否有上一頁
            /// </summary>
            public bool HasPrevious => Page > 1;

            /// <summary>
            /// 是否有下一頁
            /// </summary>
            public bool HasNext => TotalPages > Page;

            /// <summary>
            /// 列表資料
            /// </summary>
            public List<T> Items { get; set; }
        }
    }
}
