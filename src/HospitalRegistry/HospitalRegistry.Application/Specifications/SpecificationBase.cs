using HospitalRegistry.Application.Enums;
using HospitalReqistry.Domain.Entities;
using System.Linq.Expressions;

namespace HospitalRegistry.Application.Specifications
{
    public abstract class SpecificationBase<T> : ISpecification<T>
    {
        public virtual Expression<Func<T, bool>> Predicate { get; set; }
        public virtual Expression<Func<T, object>> OrderBy { get; set; }
        public virtual SortDirection OrderDirection { get; set; }
        public virtual bool IsPaginationEnabled { get; set; }
        public virtual int Skip { get; set; }
        public virtual int Take { get; set; }
    }
}
