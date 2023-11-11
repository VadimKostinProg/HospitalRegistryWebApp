using HospitalRegistry.Application.DTO;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.ServiceContracts
{
    /// <summary>
    /// Service for applying searching, pagination, sorting and filtering specifications to query.
    /// </summary>
    public interface ISpecificationsService
    {
        /// <summary>
        /// Method for applying searching, pagination, sorting and filtering specifications to query.
        /// </summary>
        /// <typeparam name="T">Entity to apply specifications.</typeparam>
        /// <param name="query">Query to apply specifications.</param>
        /// <param name="specifications">Specifications of searching, pagination, sorting and filtering to apply.</param>
        /// <returns>Query of enities with applyed specifications.</returns>
        IQueryable<T> ApplySpecifications<T>(IQueryable<T> query, Specifications specifications)
            where T : EntityBase;
    }
}
