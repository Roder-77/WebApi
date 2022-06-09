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

        TEntity? GetById(int id, Expression<Func<TEntity, bool>>? predicate = null, bool isTracking = false);

        Task Insert(TEntity entity);

        Task Update(TEntity entity);

        Task DeleteById(int id);
    }
}
