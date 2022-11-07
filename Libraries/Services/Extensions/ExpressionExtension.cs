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
    }
}
