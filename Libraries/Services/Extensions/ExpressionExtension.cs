using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Services.Extensions
{
    public static class ExpressionExtension
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right) where T : class
            => Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left.Body, Expression.Invoke(right, left.Parameters)), left.Parameters);

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right) where T : class
            => Expression.Lambda<Func<T, bool>>(Expression.OrElse(left.Body, Expression.Invoke(right, left.Parameters)), left.Parameters);

        public static Expression<Func<T, bool>>? CombineByAnd<T>(this IEnumerable<Expression<Func<T, bool>>> predicates) where T : class
        {
            if (!predicates.Any())
                return null;

            Expression<Func<T, bool>>? predicate = null;

            foreach (var item in predicates)
                predicate = predicate is null ? item : predicate.And(item);

            return predicate;
        }

        public static Action<UpdateSettersBuilder<T>> Append<T>(this Action<UpdateSettersBuilder<T>> current, Action<UpdateSettersBuilder<T>> additional)
            where T : class
        {
            Action<UpdateSettersBuilder<T>> setPropertyCalls = (y) =>
            {
                current.Invoke(y);
                additional.Invoke(y);
            };
            return setPropertyCalls;
        }

        public static Action<UpdateSettersBuilder<T>>? Combine<T>(this IEnumerable<Action<UpdateSettersBuilder<T>>> predicates)
            where T : class
        {
            if (!predicates.Any())
                return null;

            Action<UpdateSettersBuilder<T>>? predicate = null;
            foreach (var item in predicates)
                predicate = predicate is null ? item : predicate.Append(item);

            return predicate;
        }
    }
}
