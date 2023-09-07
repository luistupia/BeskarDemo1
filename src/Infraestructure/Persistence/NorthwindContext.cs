using Application.Common.Interfaces;
using Domain.Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence;

internal class NorthwindContext : DbContext, INorthwindContext
{
    public NorthwindContext(DbContextOptions<NorthwindContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaveChanges();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is AuditableEntity && e.State is EntityState.Added or EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            var entity = (AuditableEntity)entityEntry.Entity;
            switch (entityEntry)
            {
                case { State: EntityState.Added }:
                    entity.IsActive = true;
                    entity.CreatedAt = DateTime.Now;
                    break;
                case { State: EntityState.Modified }:
                    Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                    entity.ModifiedAt = DateTime.Now;
                    break;
            }
        }
    }

    //public string ConnectionString() => this.ConnectionString();
    /*public int BulkDelete<TEntity>(Expression<Func<TEntity, bool>> query) where TEntity : class =>
        Set<TEntity>().Where(query).ExecuteDelete();

    public async Task<int> BulkDeleteAsync<TEntity>(Expression<Func<TEntity, bool>> query) where TEntity : class =>
        await Set<TEntity>().Where(query).ExecuteDeleteAsync();

    public int BulkUpdate<TEntity>(Expression<Func<TEntity, bool>> query,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> expression) where TEntity : class =>
        Set<TEntity>().Where(query).ExecuteUpdate(expression);

    public async Task<int> BulkUpdateAsync<TEntity>(Expression<Func<TEntity, bool>> query,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> expression) where TEntity : class =>
        await Set<TEntity>().Where(query).ExecuteUpdateAsync(expression);*/
}