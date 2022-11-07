using Common.Extensions;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Models.DataModels;
using Services.Extensions;
using System.Data;
using System.Linq.Expressions;
using Z.BulkOperations;
using static Services.Extensions.PaginationExtension;

namespace Services.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : BaseDataModel
    {
        private readonly MemoryContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private Action<BulkOperation> _bulkOperation => options =>
        {
            options.BatchSize = 100;
            options.AutoMapOutputDirection = false;
        };

        public GenericRepository(MemoryContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public DatabaseFacade Database => _context.Database;
        public DbSet<TEntity> Table => _context.Set<TEntity>();
        public IQueryable<TEntity> TableWithoutTracking => _context.Set<TEntity>().AsNoTracking();

        private IQueryable<TEntity> Query(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? order = null,
            bool hasTracking = false)
        {
            var query = hasTracking ? Table : TableWithoutTracking;

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
                query = query
                    .Cast<ISoftDelete>()
                    .Where(x => !x.IsDeleted)
                    .Cast<TEntity>();

            if (predicate is not null)
                query = query.Where(predicate);

            if (include is not null)
                query = include(query);

            if (order is not null)
                query = order(query);

            return query;
        }

        public async Task<TEntity?> Get(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? order = null,
            bool hasTracking = false)
            => await Query(predicate, include, order, hasTracking).FirstOrDefaultAsync();

        public async Task<TEntity?> GetById(
            int id,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? order = null,
            bool hasTracking = false)
            => await Query(predicate, include, order, hasTracking).FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<TEntity>> GetList(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? order = null,
            bool hasTracking = false)
            => await Query(predicate, include, order, hasTracking).ToListAsync();

        public async Task<PaginationList<TEntity>> GetPaginationList(
            int page,
            int pageSize,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? order = null,
            bool hasTracking = false)
            => await Query(predicate, include, order, hasTracking).ToPaginationList(page, pageSize);


        public async Task Insert(TEntity entity, bool saveImmediately = true)
        {
            var entityType = typeof(TEntity);
            var nowTime = DateTime.Now.ToTimestamp();
            var member = _httpContextAccessor.HttpContext.GetMember();

            if (typeof(ICreateEntity).IsAssignableFrom(entityType))
            {
                var createEntity = (ICreateEntity)entity;
                createEntity.CreateTime = nowTime;
                createEntity.Creator = member.Id;
            }

            if (typeof(IUpdateEntity).IsAssignableFrom(entityType))
            {
                var updateEntity = (IUpdateEntity)entity;
                updateEntity.UpdateTime = nowTime;
                updateEntity.Updater = member.Id;
            }

            Table.Add(entity);

            if (saveImmediately)
                await _context.SaveChangesAsync();
        }

        public async Task InsertMultiple(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                await Insert(entity, false);

            await _context.BulkSaveChangesAsync();
        }

        public async Task Update(TEntity entity, bool saveImmediately = true)
        {
            if (typeof(IUpdateEntity).IsAssignableFrom(typeof(TEntity)))
            {
                var member = _httpContextAccessor.HttpContext.GetMember();
                var updateEntity = (IUpdateEntity)entity;
                updateEntity.UpdateTime = DateTime.Now.ToTimestamp();
                updateEntity.Updater = member.Id;
            }

            Table.Update(entity);

            if (saveImmediately)
                await _context.SaveChangesAsync();
        }

        public async Task DeleteById(int id, bool saveImmediately = true)
        {
            var item = Table.FirstOrDefault(x => x.Id == id);

            if (item == null) return;

            Table.Remove(item);

            if (saveImmediately)
                await _context.SaveChangesAsync();
        }

        public async Task BulkInsert(IEnumerable<TEntity> entities) => await _context.BulkInsertAsync(entities, _bulkOperation);

        public async Task BulkInsert(IEnumerable<TEntity> entities, Action<BulkOperation<TEntity>> options) => await _context.BulkInsertAsync(entities, options);

        public async Task BulkUpdate(IEnumerable<TEntity> entities) => await _context.BulkUpdateAsync(entities, _bulkOperation);

        public async Task BulkUpdate(IEnumerable<TEntity> entities, Action<BulkOperation<TEntity>> options) => await _context.BulkUpdateAsync(entities, options);

        public async Task BulkDelete(IEnumerable<TEntity> entities) => await _context.BulkDeleteAsync(entities, _bulkOperation);

        public async Task BulkDelete(IEnumerable<TEntity> entities, Action<BulkOperation<TEntity>> options) => await _context.BulkDeleteAsync(entities, options);

        public async Task<int> Execute(string sql, object parameter, int? commandTimeout = null, CommandType? commandType = null)
        {
            var connection = Database.GetDbConnection();
            var transaction = Database.CurrentTransaction?.GetDbTransaction();

            return await connection.ExecuteAsync(sql, parameter, transaction, commandTimeout, commandType);
        }

        public async Task<IEnumerable<T>> Query<T>(string sql, object parameter, int? commandTimeout = null, CommandType? commandType = null)
        {
            var connection = Database.GetDbConnection();
            var transaction = Database.CurrentTransaction?.GetDbTransaction();

            return await connection.QueryAsync<T>(sql, parameter, transaction, commandTimeout, commandType);
        }

        public async Task<T> QueryFirstOrDefault<T>(string sql, object parameter, int? commandTimeout = null, CommandType? commandType = null)
        {
            var connection = Database.GetDbConnection();
            var transaction = Database.CurrentTransaction?.GetDbTransaction();

            return await connection.QueryFirstOrDefaultAsync<T>(sql, parameter, transaction, commandTimeout, commandType);
        }
    }
}
