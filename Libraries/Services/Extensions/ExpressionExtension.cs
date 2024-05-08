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
            Expression<Func<T, bool>>? predicate = null;

            foreach (var item in predicates)
                predicate = predicate is null ? item : predicate.And(item);

            return predicate;
        }

        public static Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> Append<T>(
            this Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> left,
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> right) where T : class
        {
            var replace = new ReplacingExpressionVisitor(right.Parameters, new[] { left.Body });
            var combined = replace.Visit(right.Body);
            return Expression.Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(combined, left.Parameters);
        }

        public static Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>? Combine<T>(
            this IEnumerable<Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>> predicates) where T : class
        {
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>? predicate = null;

            foreach (var item in predicates)
                predicate = predicate is null ? item : predicate.Append(item);

            return predicate;
        }
    }
}
