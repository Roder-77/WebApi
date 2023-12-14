using Common.Enums;
using Models;
using Models.DataModels;
using System.Linq.Expressions;

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
        public static IOrderedQueryable<TEntity> DynamicOrderBy<TEntity>(this IQueryable<TEntity> query, SortType sortType, Expression<Func<TEntity, object>> expression)
            where TEntity : BaseDataModel
        {
            var IsSortByASC = sortType == SortType.ASC;

            if (query is IOrderedQueryable<TEntity> orderedQuery)
                return IsSortByASC ? orderedQuery.ThenBy(expression) : orderedQuery.ThenByDescending(expression);

            return IsSortByASC ? query.OrderBy(expression) : query.OrderByDescending(expression);
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
        public static IQueryable<TEntity> DynamicOrderBy<TEntity, TEnum>(this IQueryable<TEntity> query, SortCondition<TEnum>? sortCondition, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customOrder = null)
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

            return query.DynamicOrderBy(sortCondition.SortType!.Value, orderExpression);
        }

        /// <summary>
        /// 產生 Lambda 表達式
        /// </summary>
        /// <typeparam name="TEntity">資料實體</typeparam>
        /// <param name="columnPath">欄位路徑，以點區隔</param>
        /// <returns></returns>
        private static Expression<Func<TEntity, object>>? GenerateLambdaExpression<TEntity>(string columnPath)
            where TEntity : BaseDataModel
        {
            MemberExpression? member = null;
            var type = typeof(TEntity);
            // {x} =>
            var parameter = Expression.Parameter(type);

            // x.{property} or more x.{property}.{property}...
            try
            {
                foreach (var part in columnPath.Split("."))
                {
                    if (member is null)
                    {
                        member = Expression.Property(parameter, part);
                        continue;
                    }

                    member = Expression.Property(member, part);
                }
            }
            catch (Exception ex)
            {
                // 攔截無對應 property 的錯誤
                if (ex is ArgumentException)
                    return null;
            }

            // (object)x.{property}
            var conversion = Expression.Convert(member!, typeof(object));

            // x => (object)x.{property}
            return Expression.Lambda<Func<TEntity, object>>(conversion, parameter);
        }

        /// <summary>
        /// 基於 predicates 篩選資料
        /// </summary>
        /// <typeparam name="TEntity">資料實體</typeparam>
        /// <returns></returns>
        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> query, List<Expression<Func<TEntity, bool>>> predicates)
            where TEntity : BaseDataModel
        {
            var predicate = predicates.CombineByAnd();
            return predicate is null ? query : query.Where(predicate);
        }
    }
}
