using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlindBoxShop.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class, IBaseEntity
    {
        protected RepositoryContext RepositoryContext;

        public RepositoryBase(RepositoryContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public async Task<T?> FindById(Guid id, bool trackChanges)
        => await FindByCondition(e => e.Id.Equals(id), trackChanges).SingleOrDefaultAsync();

        public IQueryable<T> FindAll(bool trackChanges) =>
            !trackChanges ?
            RepositoryContext
            .Set<T>()
            .AsNoTracking() :
            RepositoryContext
            .Set<T>();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges) =>
            !trackChanges ?
            RepositoryContext
            .Set<T>()
            .Where(expression)
            .AsNoTracking() :
            RepositoryContext
            .Set<T>()
            .Where(expression);

        public virtual void Create(T entity) =>
            RepositoryContext
            .Set<T>()
            .Add(entity);

        public virtual async Task CreateAsync(T entity) =>
            await RepositoryContext
            .Set<T>()
            .AddAsync(entity);

        public virtual void Create(T[] entity) =>
            RepositoryContext
            .Set<T>()
            .AddRange(entity);

        public virtual async Task CreateAsync(T[] entity) =>
            await RepositoryContext
            .Set<T>()
            .AddRangeAsync(entity);

        public virtual void Update(T entity) =>
            RepositoryContext
            .Set<T>()
            .Update(entity);

        public virtual void Update(T[] entity) =>
            RepositoryContext
            .Set<T>()
            .UpdateRange(entity);

        public virtual void Delete(T entity) =>
            RepositoryContext
            .Set<T>()
            .Remove(entity);

        public virtual void Delete(T[] entity) =>
            RepositoryContext
            .Set<T>()
            .RemoveRange(entity);

        public async Task SaveAsync() => await RepositoryContext.SaveChangesAsync();
    }
}
