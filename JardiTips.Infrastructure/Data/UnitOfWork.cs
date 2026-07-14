using JardiTips.Application.DataAccess;
using JardiTips.Domain.Entities;
using JardiTips.Infrastructure.Data.Repositories;

namespace JardiTips.Infrastructure.Data
{
    public class UnitOfWork(EntityDbContext context) : IUnitOfWork
    {
        private readonly Dictionary<Type, object> _repositories = new();

        public IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            if (_repositories.ContainsKey(typeof(TEntity)))
            {
                return (IRepository<TEntity>)_repositories[typeof(TEntity)];
            }

            var repository = new Repository<TEntity>(context);
            _repositories.Add(typeof(TEntity), repository);
            return repository;
        }
        
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<T> ExecuteTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await action();
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        
        public void Dispose()
        {
            context.Dispose();
        }
    }
}
