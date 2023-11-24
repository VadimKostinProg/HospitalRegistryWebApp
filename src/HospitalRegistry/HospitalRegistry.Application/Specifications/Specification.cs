using HospitalRegistry.Application.Enums;
using HospitalReqistry.Domain.Entities;
using System.Linq.Expressions;

namespace HospitalRegistry.Application.Specifications
{
    internal class Specification<T> : SpecificationBase<T> where T : EntityBase
    {
        public Specification(Expression<Func<T, bool>> predicate = null,
                             Expression<Func<T, object>> orderBy = null,
                             SortDirection orderDirection = SortDirection.ASC)
        {
            this.Predicate = predicate;
            this.OrderBy = orderBy;
            this.OrderDirection = orderDirection;
        }

        public Specification(Expression<Func<T, bool>> predicate = null,
                             Expression<Func<T, object>> orderBy = null,
                             SortDirection orderDirection = SortDirection.ASC,
                             int pageSize = 25,
                             int pageNumber = 1)
        {
            this.Predicate = predicate;
            this.OrderBy = orderBy;
            this.OrderDirection = orderDirection;

            this.IsPaginationEnabled = true;

            this.Take = pageSize;
            this.Skip = (pageNumber - 1) * pageSize;
        }
    }
}