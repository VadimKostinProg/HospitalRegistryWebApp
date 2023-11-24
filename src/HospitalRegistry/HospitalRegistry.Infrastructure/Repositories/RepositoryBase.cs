using System.Linq.Expressions;
using HospitalRegistry.Application.Specifications;
using HospitalRegistry.Infrastructure.DatabaseContexts;
using HospitalReqistry.Application.RepositoryContracts;
using HospitalReqistry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistry.Infrastructure.Repositories;

public class RepositoryBase : IAsyncRepository
{
    protected readonly ApplicationContext Context;

    public RepositoryBase(ApplicationContext context)
    {
        this.Context = context;
    }
    
    public virtual async Task<IEnumerable<T>> GetAsync<T>(ISpecification<T> specification, bool disableTracking = true) where T : EntityBase
    {
        var entities = Context.Set<T>().AsQueryable();

        if (!disableTracking)
        {
            entities = entities.AsNoTracking();
        }

        entities = entities.ApplySpecifications(specification);

        return await entities.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetFilteredAsync<T>(Expression<Func<T, bool>> predicate, bool disableTracking = true) where T : EntityBase
    {
        var entities = Context.Set<T>().Where(predicate);

        if (!disableTracking)
        {
            entities = entities.AsNoTracking();
        }

        return await entities.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync<T>(Guid id, bool disableTracking = true) where T : EntityBase
    {
        var query = Context.Set<T>().AsQueryable();

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public virtual async Task<T?> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> expression, bool disableTracking = true) where T : EntityBase
    {
        var query = Context.Set<T>().AsQueryable();

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(expression);
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
        var existingEntity = await Context.Set<T>().FindAsync(entity.Id);
        if (existingEntity != null)
        {
            Context.Entry(existingEntity).CurrentValues.SetValues(entity);
            await Context.SaveChangesAsync();
        }
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