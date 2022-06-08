using Microsoft.EntityFrameworkCore;
using WebApi.DataModel;

namespace WebApi.Repositories
{
    public interface IGenericRepository<TEntity>
        where TEntity : BaseDataModel
    {
        DbSet<TEntity> Table { get; }

        IQueryable<TEntity> TableWithoutTracking { get; }

        TEntity? GetById(int id, bool isTracking = false);

        Task Insert(TEntity entity);

        Task Update(TEntity entity);

        Task DeleteById(int id);
    }
}
