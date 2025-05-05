using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Models.DataModels;
using System.Data;
using System.Linq.Expressions;

namespace Services.Repositories
{
    public interface IGenericRepository<TEntity>
        where TEntity : BaseDataModel
    {
        DatabaseFacade Database { get; }

        DbSet<TEntity> DbSetTable { get; }

        IQueryable<TEntity> Table { get; }

        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? order = null, bool hasTracking = false);

        Task Insert(TEntity entity, bool saveImmediately = true, bool setCreator = true);

        Task InsertRange(IEnumerable<TEntity> entities, bool setCreator = true);

        Task Update(TEntity entity, bool saveImmediately = true, bool setUpdater = true);

        Task UpdateRange(IEnumerable<TEntity> entities, bool setUpdater = true);

        Task ExecuteUpdateById(int? id, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls);

        Task DeleteById(int id, bool saveImmediately = true);

        Task Delete(TEntity entity, bool saveImmediately = true);

        Task DeleteRange(IEnumerable<TEntity> entities, bool saveImmediately = true);

        Task<int> Execute(string sql, object parameter, int? commandTimeout = null, CommandType? commandType = null);

        Task<IEnumerable<T>> Query<T>(string sql, object parameter, int? commandTimeout = null, CommandType? commandType = null);

        Task<T?> QueryFirstOrDefault<T>(string sql, object parameter, int? commandTimeout = null, CommandType? commandType = null);

        Task ExecuteByTransaction(Func<Task> function, string errorMessage, IsolationLevel? isolationLevel = null);
    }
}
