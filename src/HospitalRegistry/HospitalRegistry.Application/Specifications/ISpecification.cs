using HospitalRegistry.Application.Enums;
using HospitalReqistry.Domain.Entities;
using System.Linq.Expressions;

namespace HospitalRegistry.Application.Specifications
{
    /// <summary>
    /// Specification for entities.
    /// </summary>
    /// <typeparam name="T">Type of entity.</typeparam>
    public interface ISpecification<T>
    {
        /// <summary>
        /// Expression for filtering.
        /// </summary>
        Expression<Func<T, bool>> Predicate { get; set; }

        /// <summary>
        /// Expression for entity field to sort by.
        /// </summary>
        Expression<Func<T, object>> OrderBy { get; set; }

        /// <summary>
        /// Direction to sort in.
        /// </summary>
        SortDirection OrderDirection { get; set; }

        /// <summary>
        /// Flag that identifies whether the pagination is enabled.
        /// </summary>
        bool IsPaginationEnabled { get; set; }

        /// <summary>
        /// Amount to skip elements.
        /// </summary>
        int Skip { get; set; }

        /// <summary>
        /// Amount to take elements.
        /// </summary>
        int Take { get; set; }
    }
}
