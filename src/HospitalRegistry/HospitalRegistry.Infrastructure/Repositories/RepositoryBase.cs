using System.Linq.Expressions;
using HospitalRegistry.Application.Specifications;
using HospitalRegistry.Infrastructure.DatabaseContexts;
using HospitalReqistry.Application.RepositoryContracts;
using HospitalReqistry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistry.Infrastructure.Repositories;

public class RepositoryBase : IAsyncRepository
{
    protected readonly ApplicationContext context;

    public RepositoryBase(ApplicationContext context)
    {
        this.context = context;
    }

    public virtual async Task<long> CountAsync<T>() where T : EntityBase
    {
        return await context.Set<T>().CountAsync();
    }

    public virtual async Task<long> CountAsync<T>(Expression<Func<T, bool>> predicate) where T : EntityBase
    {
        return await context.Set<T>().CountAsync(predicate);
    }

    public virtual async Task<IEnumerable<T>> GetAsync<T>(ISpecification<T> specification, bool disableTracking = false) where T : EntityBase
    {
        var entities = context.Set<T>().AsQueryable();

        if (disableTracking)
        {
            entities = entities.AsNoTracking();
        }

        entities = entities.ApplySpecifications(specification);

        return await entities.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetFilteredAsync<T>(Expression<Func<T, bool>> predicate, bool disableTracking = false) where T : EntityBase
    {
        var entities = context.Set<T>().Where(predicate);

        if (disableTracking)
        {
            entities = entities.AsNoTracking();
        }

        return await entities.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync<T>(Guid id, bool disableTracking = false) where T : EntityBase
    {
        var query = context.Set<T>().AsQueryable();

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public virtual async Task<T?> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> expression, bool disableTracking = false) where T : EntityBase
    {
        var query = context.Set<T>().AsQueryable();

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(expression);
    }

    public virtual async Task<bool> ContainsAsync<T>(Expression<Func<T, bool>> expression) where T : EntityBase
    {
        return await context.Set<T>().CountAsync(expression) > 0;
    }

    public virtual async Task AddAsync<T>(T entity) where T : EntityBase
    {
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync<T>(T entity) where T : EntityBase
    {
        var existingEntity = await context.Set<T>().FindAsync(entity.Id);
        if (existingEntity != null)
        {
            context.Entry(existingEntity).CurrentValues.SetValues(entity);
            await context.SaveChangesAsync();
        }
    }

    public virtual async Task<bool> DeleteAsync<T>(Guid id) where T : EntityBase
    {
        var entityToDelete = await context.Set<T>().FindAsync(id);

        if (entityToDelete is null)
        {
            return false;
        }

        context.Set<T>().Remove(entityToDelete);
        await context.SaveChangesAsync();

        return true;
    }

    public virtual async Task<int> DeleteRangeAsync<T>(IEnumerable<T> entities) where T : EntityBase
    {
        context.Set<T>().RemoveRange(entities);
        return await context.SaveChangesAsync();
    }
}