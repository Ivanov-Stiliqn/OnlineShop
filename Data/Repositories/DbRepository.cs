using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class DbRepository<TEntity>: IRepository<TEntity>, IDisposable
        where TEntity : class 
    {
        private readonly ApplicationContext db;
        private readonly DbSet<TEntity> dbSet;

        public DbRepository(ApplicationContext db)
        {
            this.db = db;
            this.dbSet = this.db.Set<TEntity>();
        }

        public IQueryable<TEntity> All()
        {
            return this.dbSet;
        }

        public async Task AddAsync(TEntity entity)
        {
            await this.dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await this.dbSet.AddRangeAsync(entities);
        }

        public void Update(TEntity entity)
        {
            this.dbSet.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            this.dbSet.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await this.db.SaveChangesAsync();
        }

        public void Dispose()
        {
            this.db?.Dispose();
        }
    }
}
