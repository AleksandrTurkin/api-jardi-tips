using JardiTips.Domain.Entities;

namespace JardiTips.Application.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        Task<T> ExecuteTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken);
    }
}
