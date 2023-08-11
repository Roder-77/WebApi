using Common.Enums;
using Common.Extensions;
using System.ComponentModel;
using System.Text.Json.Serialization;

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
        /// 每頁筆數
        /// </summary>
        [DefaultValue(20)]
        public int PageSize { get; set; } = 20;
    }

    public class SortCondition<TEnum> where TEnum : struct
    {
        /// <summary>
        /// 排序欄位
        /// </summary>
        public TEnum? SortColumn { get; set; }

        /// <summary>
        /// 排序類型
        /// </summary>
        public SortType? SortType { get; set; }

        /// <summary>
        /// 欄位名稱
        /// </summary>
        [JsonIgnore]
        public string ColumnName => !SortColumn.HasValue ? string.Empty : SortColumn.Value.GetAttributeContent(AttributeType.Description);

        [JsonIgnore]
        public bool HasValue => SortColumn.HasValue && SortType.HasValue;
    }
}
