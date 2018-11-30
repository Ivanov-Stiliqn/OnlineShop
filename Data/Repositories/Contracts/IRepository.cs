using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories.Contracts
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> All();

        Task AddAsync(TEntity entity);

        void Delete(TEntity entity);

        Task<int> SaveChangesAsync();
    }
}
