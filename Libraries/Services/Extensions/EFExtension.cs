using Common.Enums;
using Models;
using Models.DataModels;
using System.Linq.Expressions;
using System.Reflection;

namespace Services.Extensions
{
    public static class EFExtension
    {
        /// <summary>
        /// 欄位排序
        /// </summary>
        /// <typeparam name="TEntity">資料實體</typeparam>
        /// <param name="query">query</param>
        /// <param name="sortType">排序類型</param>
        /// <param name="expression">排序表達式</param>
        /// <returns></returns>
        public static IOrderedQueryable<TEntity> Order<TEntity>(this IQueryable<TEntity> query, SortType sortType, Expression<Func<TEntity, object>> expression)
            where TEntity : BaseDataModel
        {
            if (query is IOrderedQueryable<TEntity>)
            {
                var orderedQuery = (IOrderedQueryable<TEntity>)query;
                return sortType == SortType.ASC ? orderedQuery.ThenBy(expression) : orderedQuery.ThenByDescending(expression);
            }

            return sortType == SortType.ASC ? query.OrderBy(expression) : query.OrderByDescending(expression);
        }

        /// <summary>
        /// 欄位排序
        /// </summary>
        /// <typeparam name="TEntity">資料實體</typeparam>
        /// <typeparam name="TEnum">實體欄位列舉</typeparam>
        /// <param name="query">query</param>
        /// <param name="orderedCondition">排序條件</param>
        /// <returns></returns>
        public static IQueryable<TEntity> Order<TEntity, TEnum>(this IQueryable<TEntity> query, OrderedCondition<TEnum>? orderedCondition)
            where TEntity : BaseDataModel
            where TEnum : Enum
        {
            if (orderedCondition is null)
                return query;

            var orderExpression = GetOrderByExpression<TEntity>(orderedCondition.PropertyName);
            if (orderExpression is null)
                return query;

            return query.Order(orderedCondition.SortType, orderExpression);
        }

        /// <summary>
        /// 多欄位排序
        /// </summary>
        /// <typeparam name="TEntity">資料實體</typeparam>
        /// <typeparam name="TEnum">實體欄位列舉</typeparam>
        /// <param name="query">query</param>
        /// <param name="orderedConditions">排序條件列表</param>
        /// <returns></returns>
        public static IQueryable<TEntity> Order<TEntity, TEnum>(this IQueryable<TEntity> query, List<OrderedCondition<TEnum>>? orderedConditions)
            where TEntity : BaseDataModel
            where TEnum : Enum
        {
            if (orderedConditions is null || !orderedConditions.Any())
                return query;

            IOrderedQueryable<TEntity>? orderedQuery = null;

            foreach (var condition in orderedConditions)
            {
                var orderExpression = GetOrderByExpression<TEntity>(condition.PropertyName);
                if (orderExpression is null)
                    continue;

                orderedQuery = orderedQuery is null
                        ? query.Order(condition.SortType, orderExpression)
                        : orderedQuery.Order(condition.SortType, orderExpression);
            }

            if (orderedQuery is null)
                return query;

            return orderedQuery;
        }

        /// <summary>
        /// 取得排序表達式
        /// </summary>
        /// <typeparam name="TEntity">資料實體</typeparam>
        /// <param name="name">欄位名稱</param>
        /// <returns></returns>
        private static Expression<Func<TEntity, object>>? GetOrderByExpression<TEntity>(string name) where TEntity : BaseDataModel
        {
            var type = typeof(TEntity);
            var propertyInfo = type.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo is null)
                return null;

            var parameter = Expression.Parameter(type);
            var member = Expression.Property(parameter, propertyInfo);
            var conversion = Expression.Convert(member, typeof(object));

            return Expression.Lambda<Func<TEntity, object>>(conversion, parameter);
        }
    }
}
