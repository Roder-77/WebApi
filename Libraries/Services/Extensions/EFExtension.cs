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
        /// <param name="sortCondition">排序條件</param>
        /// <param name="customOrder">客製排序 (排序為子表欄位時使用)</param>
        /// <returns></returns>
        public static IQueryable<TEntity> Order<TEntity, TEnum>(this IQueryable<TEntity> query, SortCondition<TEnum>? sortCondition, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customOrder = null)
            where TEntity : BaseDataModel
            where TEnum : struct
        {
            if (sortCondition is null || !sortCondition.HasValue)
                return query;

            var orderExpression = GenerateLambdaExpression<TEntity>(sortCondition.ColumnName);
            if (orderExpression is null)
            {
                if (customOrder is null)
                    return query;

                return customOrder(query);
            }

            return query.Order(sortCondition.SortType!.Value, orderExpression);
        }

        /// <summary>
        /// 取得排序表達式
        /// </summary>
        /// <typeparam name="TEntity">資料實體</typeparam>
        /// <param name="name">欄位名稱</param>
        /// <returns></returns>
        private static Expression<Func<TEntity, object>>? GenerateLambdaExpression<TEntity>(string name) where TEntity : BaseDataModel
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
