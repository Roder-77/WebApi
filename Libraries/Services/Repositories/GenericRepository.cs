using Common.Enums;
using Common.Extensions;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models.DataModels;
using Services.Extensions;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Services.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : BaseDataModel
    {
        private readonly DataContext _context;
        private readonly HttpContext _httpContext;
        private readonly ClaimsPrincipal _user;

        private readonly IServiceProvider _serviceProvider;

        public GenericRepository(DataContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _httpContext = serviceProvider.SetDefaultHttpContext().HttpContext!;
            _user = _httpContext.User;
        }

        private int _id => int.Parse(_user.FindFirstValue("Id")!);

        public DatabaseFacade Database => _context.Database;
        public DbSet<TEntity> DbSetTable => _context.Set<TEntity>();
        public IQueryable<TEntity> Table => _context.Set<TEntity>().AsNoTrackingWithIdentityResolution();

        public IQueryable<TEntity> Query(
            Expression<Func<TEntity, bool>>? predicate = null,
            bool hasTracking = false)
        {
            var query = hasTracking ? DbSetTable : Table;

            if (typeof(ISoftDeleteEntity).IsAssignableFrom(typeof(TEntity)))
                query = query
                    .Cast<ISoftDeleteEntity>()
                    .Where(x => !x.IsDeleted)
                    .Cast<TEntity>();

            if (typeof(IActiveEntity).IsAssignableFrom(typeof(TEntity)))
                query = query
                    .Cast<IActiveEntity>()
                    .Where(x => x.IsActive)
                    .Cast<TEntity>();

            if (predicate is not null)
                query = query.Where(predicate);

            return query;
        }

        public async Task Insert(TEntity entity, bool saveImmediately = true, bool setCreator = true)
        {
            var nowTime = DateTime.Now.ToTimestamp();
            var memberId = setCreator ? _id : 1;

            if (entity is ICreateEntity createEntity)
            {
                if (entity is ICreatorEntity creatorEntity)
                    creatorEntity.Creator = memberId;

                if (createEntity.CreateTime == default)
                    createEntity.CreateTime = nowTime;
            }

            if (entity is IUpdateEntity updateEntity)
            {
                if (entity is IUpdaterEntity updaterEntity)
                    updaterEntity.Updater = memberId;

                if (updateEntity.UpdateTime == default)
                    updateEntity.UpdateTime = nowTime;
            }

            _context.Add(entity);

            if (saveImmediately)
                await _context.SaveChangesAsync();
        }

        public async Task InsertRange(IEnumerable<TEntity> entities, bool setCreator = true)
        {
            foreach (var entity in entities)
                await Insert(entity, false, setCreator);

            await _context.SaveChangesAsync();
        }

        public async Task Update(TEntity entity, bool saveImmediately = true, bool setUpdater = true)
        {
            if (entity is IUpdateEntity updateEntity)
            {
                if (entity is IUpdaterEntity updaterEntity)
                    updaterEntity.Updater = setUpdater ? _id : 1;

                updateEntity.UpdateTime = DateTime.Now.ToTimestamp();
            }

            _context.Update(entity);

            if (saveImmediately)
                await _context.SaveChangesAsync();
        }

        public async Task UpdateRange(IEnumerable<TEntity> entities, bool setUpdater = true)
        {
            if (entities is IEnumerable<IUpdateEntity> updateEntities)
            {
                foreach (IUpdateEntity updateEntity in updateEntities)
                {
                    if (updateEntity is IUpdaterEntity updaterEntity)
                        updaterEntity.Updater = setUpdater ? _id : 1;

                    updateEntity.UpdateTime = DateTime.Now.ToTimestamp();
                }
            }

            _context.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteById(int id, bool saveImmediately = true)
        {
            var item = DbSetTable.FirstOrDefault(x => x.Id == id);

            if (item == null) return;

            _context.Remove(item);

            if (saveImmediately)
                await _context.SaveChangesAsync();
        }

        public async Task Delete(TEntity entity, bool saveImmediately = true)
        {
            _context.Remove(entity);

            if (saveImmediately)
                await _context.SaveChangesAsync();
        }

        public async Task DeleteRange(IEnumerable<TEntity> entities, bool saveImmediately = true)
        {
            DbSetTable.RemoveRange(entities);

            if (saveImmediately)
                await _context.SaveChangesAsync();
        }

        //public async Task ExecuteUpdateById(int? id, Action<UpdateSettersBuilder<TEntity>> setPropertyCalls)
        //{
        //    if (typeof(IUpdateEntity).IsAssignableFrom(typeof(TEntity)))
        //    {
        //        var now = DateTime.Now;
        //        setPropertyCalls = setPropertyCalls.Append(y => y.SetProperty(z => ((IUpdateEntity)z).UpdateTime, _ => now));

        //        if (typeof(IUpdaterEntity).IsAssignableFrom(typeof(TEntity)))
        //            setPropertyCalls = setPropertyCalls.Append(y => y.SetProperty(z => ((IUpdaterEntity)z).Updater, _ => _id));
        //    }

        //    await DbSetTable.Where(x => x.Id == id).ExecuteUpdateAsync(setPropertyCalls);
        //}

        private (DbConnection connection, DbTransaction? transaction) GetDbConnectionAndTransaction()
        {
            var connection = Database.GetDbConnection();
            var transaction = Database.CurrentTransaction?.GetDbTransaction();
            return (connection, transaction);
        }

        public async Task<int> Execute(string sql, object? parameter = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var (connection, transaction) = GetDbConnectionAndTransaction();
            return await connection.ExecuteAsync(sql, parameter, transaction, commandTimeout, commandType);
        }

        public async Task<List<T>> Query<T>(string sql, object? parameter = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var (connection, transaction) = GetDbConnectionAndTransaction();
            return (await connection.QueryAsync<T>(sql, parameter, transaction, commandTimeout, commandType)).ToList();
        }

        public async Task<T?> QueryFirstOrDefault<T>(string sql, object? parameter = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var (connection, transaction) = GetDbConnectionAndTransaction();
            return await connection.QueryFirstOrDefaultAsync<T>(sql, parameter, transaction, commandTimeout, commandType);
        }

        public async Task ExecuteByTransaction(Func<Task> function, string errorMessage, IsolationLevel? isolationLevel = null)
        {
            using var transaction = isolationLevel.HasValue
                ? await Database.BeginTransactionAsync(isolationLevel.GetValueOrDefault())
                : await Database.BeginTransactionAsync();

            try
            {
                await function();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                using var scope = _serviceProvider.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<GenericRepository<TEntity>>>();
                logger.LogError(ex, errorMessage);

                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<long> GetNextSequenceId(DbSeqence sequence)
        {
            var sql = $"SELECT NEXT VALUE FOR @sequenceName";
            return await QueryFirstOrDefault<long>(sql, new { sequenceName = sequence.ToString() });
        }
    }
}
