using JardiTips.Application.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JardiTips.Infrastructure.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext Context;
        protected readonly DbSet<T> DbSet;

        public Repository(DbContext context)
        {
            Context = context;
            DbSet = Context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken)
        {
            return await DbSet.FindAsync(id, cancellationToken);
        }

        public T? GetById(object id)
        {
            return DbSet.Find(id);
        }

        public async Task<T?> GetByIdAsync(IQueryable<T> query, object id, CancellationToken cancellationToken)
        {
            var type = typeof(T);
            var entityType = Context.Model.FindEntityType(type);
            var keyProperty = entityType?.FindPrimaryKey()?.Properties.SingleOrDefault();

            if (keyProperty != null)
            {
                var item = await query.FirstOrDefaultAsync(x => EF.Property<object>(x, keyProperty.Name) == id, cancellationToken);

                return item;
            }

            throw new NotImplementedException($"GetByIdAsync not implemented for {type.FullName}");
        }

        public IQueryable<T> GetByPrimaryKey(object key)
        {
            var entityType = Context.Model.FindEntityType(typeof(T))
                ?? throw new InvalidOperationException($"Entity {typeof(T).Name} not found");

            var primaryKey = entityType.FindPrimaryKey()
                ?? throw new InvalidOperationException($"Entity {typeof(T).Name} doesn't have a primary key");

            if (primaryKey.Properties.Count != 1)
                throw new NotSupportedException("Composite primary keys are not supported ");

            var keyProperty = primaryKey.Properties[0];

            return DbSet.Where(e =>
                EF.Property<object>(e, keyProperty.Name).Equals(key));
        }


        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await DbSet.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await DbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
        }

        public void Update(T entity)
        {
            DbSet.Update(entity);
        }

        public void UpdateRange(params T[] entities)
        {
            DbSet.UpdateRange(entities);
        }

        public void SetValuesEntry(T existing, T updateItem)
        {
            Context.Entry(existing).CurrentValues.SetValues(updateItem);
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public void Remove(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                DbSet.Remove(entity);
            }
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await DbSet.AnyAsync(predicate, cancellationToken);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public IQueryable<T> GetAll()
        {
            return DbSet;
        }
    }
}
