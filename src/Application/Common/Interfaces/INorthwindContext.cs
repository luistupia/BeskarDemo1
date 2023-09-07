using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface INorthwindContext
{
    DbSet<Customer> Customers { get; set; }

    // int BulkDelete<TEntity>(Expression<Func<TEntity, bool>> query) where TEntity : class;
    // Task<int> BulkDeleteAsync<TEntity>(Expression<Func<TEntity, bool>> query) where TEntity : class;
    // int BulkUpdate<TEntity>(Expression<Func<TEntity, bool>> query,
    //     Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> expression) where TEntity : class;
    // Task<int> BulkUpdateAsync<TEntity>(Expression<Func<TEntity, bool>> query,
    //     Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> expression) where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}