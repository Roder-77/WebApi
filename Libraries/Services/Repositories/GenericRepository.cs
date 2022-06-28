using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Models.DataModels;
using Models.Extensions;
using System.Data;
using System.Linq.Expressions;

namespace Services.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : BaseDataModel
    {
        private readonly MemoryContext _context;

        public GenericRepository(MemoryContext context)
        {
            _context = context;
        }

        public DbSet<TEntity> Table => _context.Set<TEntity>();
        public IQueryable<TEntity> TableWithoutTracking => _context.Set<TEntity>().AsNoTracking();

        public TEntity? GetById(int id, Expression<Func<TEntity, bool>>? predicate = null, bool hasTracking = false)
        {
            var query = Table.Where(x => x.Id == id);

            if (predicate != null)
                query = query.Where(predicate);

            if (hasTracking) 
                return query.FirstOrDefault();

            return query.AsNoTracking().FirstOrDefault();
        }

        public async Task Insert(TEntity entity, bool saveImmediately = true)
        {
            var entityType = typeof(TEntity);
            var nowTime = DateTime.Now.ToTimestamp();

            if (typeof(ICreateEntity).IsAssignableFrom(entityType))
            {
                var createEntity = (ICreateEntity)entity;
                createEntity.CreateTime = nowTime;
                createEntity.Creator = default; // current user id
            }

            if (typeof(IUpdateEntity).IsAssignableFrom(entityType))
            {
                var updateEntity = (IUpdateEntity)entity;
                updateEntity.UpdateTime = nowTime;
                updateEntity.Updater = default; // current user id
            }

            Table.Add(entity);

            if (saveImmediately)
                await _context.SaveChangesAsync();
        }

        public async Task Update(TEntity entity, bool saveImmediately = true)
        {
            if (typeof(IUpdateEntity).IsAssignableFrom(typeof(TEntity)))
            {
                var updateEntity = (IUpdateEntity)entity;
                updateEntity.UpdateTime = DateTime.Now.ToTimestamp();
                updateEntity.Updater = default; // current user id
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

        public async Task<int> Execute(string sql, object parameter, int? commandTimeout = null, CommandType? commandType = null)
        {
            var connection = _context.Database.GetDbConnection();
            var transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

            return await connection.ExecuteAsync(sql, parameter, transaction, commandTimeout, commandType);
        }

        public async Task<IEnumerable<T>> Query<T>(string sql, object parameter, int? commandTimeout = null, CommandType? commandType = null)
        {
            var connection = _context.Database.GetDbConnection();
            var transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

            return await connection.QueryAsync<T>(sql, parameter, transaction, commandTimeout, commandType);
        }

        public async Task<T> QueryFirstOrDefault<T>(string sql, object parameter, int? commandTimeout = null, CommandType? commandType = null)
        {
            var connection = _context.Database.GetDbConnection();
            var transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

            return await connection.QueryFirstOrDefaultAsync<T>(sql, parameter, transaction, commandTimeout, commandType);
        }
    }
}
