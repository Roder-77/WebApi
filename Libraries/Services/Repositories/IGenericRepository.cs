using Microsoft.EntityFrameworkCore;
using Models.DataModels;
using System.Linq.Expressions;

namespace Services.Repositories
{
    public interface IGenericRepository<TEntity>
        where TEntity : BaseDataModel
    {
        DbSet<TEntity> Table { get; }

        IQueryable<TEntity> TableWithoutTracking { get; }

        TEntity? GetById(int id, Expression<Func<TEntity, bool>>? predicate = null, bool hasTracking = false);

        Task Insert(TEntity entity, bool saveImmediately = true);

        Task Update(TEntity entity, bool saveImmediately = true);

        Task DeleteById(int id, bool saveImmediately = true);
    }
}
