using Microsoft.Extensions.Logging;
using Models;
using Models.DataModels;
using Services.Extensions;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace Services
{
    public class CommonService
    {
        private readonly ILogger<CommonService> _logger;

        public CommonService(ILogger<CommonService> logger)
        {
            _logger = logger;
        }

        //private Expression<Func<TEntity, object>>? GetOrderByExpression<TEntity>(string propertyName) where TEntity : BaseDataModel
        //{
        //    var type = typeof(TEntity);
        //    var propertyInfo = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        //    if (propertyInfo is null)
        //    {
        //        _logger.LogWarning($"{nameof(GetOrderByExpression)}, 沒有對應欄位，無法篩選 property name: {propertyName}, data model: {nameof(TEntity)}");
        //        return null;
        //    }

        //    var parameter = Expression.Parameter(type);
        //    var member = Expression.Property(parameter, propertyInfo);
        //    var conversion = Expression.Convert(member, typeof(object));

        //    return Expression.Lambda<Func<TEntity, object>>(conversion, parameter);
        //}

        //public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? GenerateOrderedFunc<TEntity, TEnum>(OrderedCondition<TEnum> orderedCondition)
        //    where TEntity : BaseDataModel
        //    where TEnum : Enum
        //{
        //    var orderExpression = GetOrderByExpression<TEntity>(orderedCondition.PropertyName);
        //    if (orderExpression is null)
        //        return null;

        //    return x => x.Order(orderedCondition.SortType, orderExpression);
        //}

        //public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? GenerateOrderedFunc<TEntity, TEnum>(List<OrderedCondition<TEnum>> orderedConditions)
        //    where TEntity : BaseDataModel
        //    where TEnum : Enum
        //{
        //    var expressions = new Dictionary<SortType, Expression<Func<TEntity, object>>>();

        //    foreach (var condition in orderedConditions)
        //    {
        //        var orderExpression = GetOrderByExpression<TEntity>(condition.PropertyName);
        //        if (orderExpression is null)
        //            continue;

        //        expressions.Add(condition.SortType, orderExpression);
        //    }

        //    if (!expressions.Any())
        //    {
        //        _logger.LogWarning($"{nameof(GenerateOrderedFunc)}, 無對應任何表達式 conditions: {JsonSerializer.Serialize(orderedConditions)}");
        //        return null;
        //    }

        //    return x => {
        //        IOrderedQueryable<TEntity>? order = null;

        //        foreach (var expression in expressions)
        //            order = order is null
        //                ? x.Order(expression.Key, expression.Value)
        //                : order.Order(expression.Key, expression.Value);

        //        return order!;
        //    };
        //}
    }
}
