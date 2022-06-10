#nullable disable

namespace Models.Extensions
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

        public class PaginationList<T> where T : class
        {
            public int Page { get; set; }

            public int PageSize { get; set; }

            public int TotalCount { get; set; }

            public int TotalPages
            {
                get
                {
                    var totalPages = TotalCount / PageSize;

                    if (TotalCount % PageSize != 0)
                        totalPages += 1;

                    return totalPages;
                }
            }

            public bool HasNext => TotalPages - Page > 0;

            public List<T> Items { get; set; }
        }
    }
}
