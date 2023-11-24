using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.Specifications
{
    public static class SpecificationHandler
    {
        public static IQueryable<T> ApplySpecifications<T>(this IQueryable<T> query, ISpecification<T> specification)
        {
            if (specification.Predicate is not null)
            {
                query = query.Where(specification.Predicate);
            }

            if (specification.OrderBy is not null)
            {
                switch (specification.OrderDirection)
                {
                    case Enums.SortDirection.ASC:
                        query = query.OrderBy(specification.OrderBy);
                        break;
                    case Enums.SortDirection.DESC:
                        query = query.OrderByDescending(specification.OrderBy);
                        break;
                }
            }

            if (specification.IsPaginationEnabled)
            {
                query = query.Skip(specification.Skip)
                         .Take(specification.Take);
            }

            return query;
        }
    }
}
