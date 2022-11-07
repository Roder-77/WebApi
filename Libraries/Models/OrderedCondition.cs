using Common.Enums;

namespace Models
{
    public class OrderedCondition<T> where T : Enum
    {
        public OrderedCondition()
        { }

        public OrderedCondition(T sortProperty, SortType sortType)
        {
            PropertyName = sortProperty.ToString();
            SortType = sortType;
        }

        public string PropertyName { get; set; }

        public SortType SortType { get; set; }
    }
}
