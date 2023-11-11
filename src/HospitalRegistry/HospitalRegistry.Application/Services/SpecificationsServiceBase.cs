using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.ServiceContracts;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.Services
{
    public abstract class SpecificationsServiceBase : ISpecificationsService
    {
        public virtual IQueryable<T> ApplySpecifications<T>(IQueryable<T> query, Specifications specifications)
        {
            query = ApplyFiltering(query, specifications);
            query = ApplySorting(query, specifications);
            query = ApplySearching(query, specifications);
            query = ApplyPagination(query, specifications);

            return query;
        }

        protected abstract IQueryable<T> ApplyFiltering<T>(IQueryable<T> query, Specifications specifications);
        protected abstract IQueryable<T> ApplySorting<T>(IQueryable<T> query, Specifications specifications);
        protected abstract IQueryable<T> ApplySearching<T>(IQueryable<T> query, Specifications specifications);
        protected abstract IQueryable<T> ApplyPagination<T>(IQueryable<T> query, Specifications specifications);
    }
}
