using System.ComponentModel;

namespace Models
{
    public class PageModel
    {
        /// <summary>
        /// 當前頁數
        /// </summary>
        [DefaultValue(1)]
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每頁數量
        /// </summary>
        [DefaultValue(20)]
        public int PageSize { get; set; } = 20;
    }
}
