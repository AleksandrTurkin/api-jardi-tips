using System.Linq.Expressions;

namespace JardiTips.Application.DataAccess
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken);

        T? GetById(object id);

        Task<T?> GetByIdAsync(IQueryable<T> query, object id, CancellationToken cancellationToken);

        IQueryable<T> GetByPrimaryKey(object Id);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

        Task AddAsync(T entity, CancellationToken cancellationToken);

        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

        void Update(T entity);

        void UpdateRange(params T[] entities);

        void SetValuesEntry(T existing, T updateItem);

        void Remove(T entity);

        void Remove(IEnumerable<T> entities);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

        IQueryable<T> Where(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAll();
    }
}
