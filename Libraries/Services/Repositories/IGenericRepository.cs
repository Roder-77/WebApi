using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Models.DataModels;
using System.Data;
using System.Linq.Expressions;
using Z.BulkOperations;
using static Services.Extensions.PaginationExtension;

namespace Services.Repositories
{
    public interface IGenericRepository<TEntity>
        where TEntity : BaseDataModel
    {
        DatabaseFacade Database { get; }

        DbSet<TEntity> DbSetTable { get; }

        IQueryable<TEntity> Table { get; }

        Task<TEntity?> Get(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? order = null, bool hasTracking = false);

        Task<TEntity?> GetById(int id, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? order = null, bool hasTracking = false);

        Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? order = null, bool hasTracking = false);

        Task<PaginationList<TEntity>> GetPaginationList(int page, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? order = null, bool hasTracking = false);

        Task Insert(TEntity entity, bool saveImmediately = true, bool setCreator = true);

        Task InsertRange(IEnumerable<TEntity> entities, bool setCreator = true);

        Task Update(TEntity entity, bool saveImmediately = true, bool setUpdater = true);

        Task UpdateRange(IEnumerable<TEntity> entities, bool setUpdater = true);

        Task DeleteById(int id, bool saveImmediately = true);

        Task Delete(TEntity entity, bool saveImmediately = true);

        Task DeleteRange(IEnumerable<TEntity> entities, bool saveImmediately = true);

        Task<int> SaveChanges();

        Task BulkInsert(IEnumerable<TEntity> entities);

        Task BulkInsert(IEnumerable<TEntity> entities, Action<BulkOperation<TEntity>> options);

        Task BulkUpdate(IEnumerable<TEntity> entities);

        Task BulkUpdate(IEnumerable<TEntity> entities, Action<BulkOperation<TEntity>> options);

        Task BulkDelete(IEnumerable<TEntity> entities);

        Task BulkDelete(IEnumerable<TEntity> entities, Action<BulkOperation<TEntity>> options);

        Task<int> Execute(string sql, object parameter, int? commandTimeout = null, CommandType? commandType = null);

        Task<IEnumerable<T>> Query<T>(string sql, object parameter, int? commandTimeout = null, CommandType? commandType = null);

        Task<T> QueryFirstOrDefault<T>(string sql, object parameter, int? commandTimeout = null, CommandType? commandType = null);
    }
}
