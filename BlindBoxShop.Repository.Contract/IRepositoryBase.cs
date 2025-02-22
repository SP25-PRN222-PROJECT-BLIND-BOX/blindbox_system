using System.Linq.Expressions;

namespace BlindBoxShop.Repository.Contract
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll(bool trackChanges);

        Task<T?> FindById(Guid id, bool trackChanges);

        IQueryable<T> FindByCondition(
            Expression<Func<T, bool>> expression,
            bool trackChanges);

        void Create(T entity);

        void Create(T[] entity);

        Task CreateAsync(T entity);

        Task CreateAsync(T[] entity);

        void Update(T entity);

        void Update(T[] entity);

        void Delete(T entity);

        void Delete(T[] entity);

        Task SaveAsync();
    }
}
