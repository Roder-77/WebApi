using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class PageModel
    {
        /// <summary>
        /// 當前頁數
        /// </summary>
        [Required]
        public int Page { get; set; }

        /// <summary>
        /// 每頁數量
        /// </summary>
        [Required]
        public int Quantity { get; set; }
    }
}
