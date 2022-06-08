using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using WebApi.DataModel;

namespace WebApi.Repositories
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

        public TEntity? GetById(int id, bool isTracking = false)
        {
            var query = Table.Where(x => x.Id == id);

            if (isTracking) return query.FirstOrDefault();
            return query.AsNoTracking().FirstOrDefault();
        }

        public async Task Insert(TEntity entity)
        {
            Table.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TEntity entity)
        {
            Table.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteById(int id)
        {
            var item = Table.FirstOrDefault(x => x.Id == id);

            if (item == null) return;

            Table.Remove(item);
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

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameter, int? commandTimeout = null, CommandType? commandType = null)
        {
            var connection = _context.Database.GetDbConnection();
            var transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

            return await connection.QueryFirstOrDefaultAsync<T>(sql, parameter, transaction, commandTimeout, commandType);
        }
    }
}
