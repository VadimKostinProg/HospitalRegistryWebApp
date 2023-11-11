using System.Linq.Expressions;
using HospitalRegistry.Infrastructure.DatabaseContexts;
using HospitalReqistry.Domain.Entities;
using HospitalReqistry.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistry.Infrastructure.Repositories;

public class RepositoryBase : IAsyncRepository
{
    protected readonly ApplicationContext Context;

    public RepositoryBase(ApplicationContext context)
    {
        this.Context = context;
    }
    
    public virtual Task<IQueryable<T>> GetAllAsync<T>(bool disableTracking = true) where T : EntityBase
    {
        var entities = Context.Set<T>().AsQueryable();

        if (!disableTracking)
        {
            entities = entities.AsNoTracking();
        }

        return Task.FromResult(entities);
    }

    public virtual Task<IQueryable<T>> GetFilteredAsync<T>(Expression<Func<T, bool>> predicate, bool disableTracking = true) where T : EntityBase
    {
        var entities = Context.Set<T>().Where(predicate);

        if (!disableTracking)
        {
            entities = entities.AsNoTracking();
        }

        return Task.FromResult(entities);
    }

    public virtual async Task<T?> GetByIdAsync<T>(Guid id) where T : EntityBase
    {
        return await Context.Set<T>().FindAsync(id);
    }

    public virtual async Task<T?> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> expression) where T : EntityBase
    {
        return await Context.Set<T>().FirstOrDefaultAsync(expression);
    }

    public virtual async Task<bool> ContainsAsync<T>(Expression<Func<T, bool>> expression) where T : EntityBase
    {
        return (await Context.Set<T>().Where(expression).ToListAsync()).Count > 0;
    }

    public virtual async Task AddAsync<T>(T entity) where T : EntityBase
    {
        await Context.Set<T>().AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync<T>(T entity) where T : EntityBase
    {
        Context.Entry(entity).State = EntityState.Modified;
        await Context.SaveChangesAsync();
    }

    public virtual async Task<bool> DeleteAsync<T>(Guid id) where T : EntityBase
    {
        var entityToDelete = await Context.Set<T>().FindAsync(id);

        if (entityToDelete is null)
        {
            return false;
        }

        Context.Set<T>().Remove(entityToDelete);
        await Context.SaveChangesAsync();

        return true;
    }

    public virtual async Task<int> DeleteRangeAsync<T>(IEnumerable<T> entities) where T : EntityBase
    {
        Context.Set<T>().RemoveRange(entities);
        return await Context.SaveChangesAsync();
    }
}