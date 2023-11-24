using HospitalRegistry.Application.Enums;
using HospitalReqistry.Domain.Entities;
using System.Linq.Expressions;

namespace HospitalRegistry.Application.Specifications
{
    public class SpecificationBuilder<T>
    {
        private readonly List<Expression<Func<T, bool>>> _filters = new();
        private Expression<Func<T, object>> _orderBy;
        private SortDirection _orderDirection;
        private bool _isPaginationEnabled;
        private int _pageSize;
        private int _pageNumber;

        public SpecificationBuilder<T> With(Expression<Func<T, bool>> predicate)
        {
            this._filters.Add(predicate);

            return this;
        }

        public SpecificationBuilder<T> OrderBy(Expression<Func<T, object>> orderBy, SortDirection orderDirection)
        {
            this._orderBy = orderBy;
            this._orderDirection = orderDirection;

            return this;
        }

        public SpecificationBuilder<T> WithPagination(int pageSize, int pageNumber)
        {
            this._isPaginationEnabled = true;

            this._pageSize = pageSize;
            this._pageNumber = pageNumber;

            return this;
        }

        public ISpecification<T> Build()
        {

            var specification = !this._isPaginationEnabled ?
                new Specification<T>(this.GetPredicate(), this._orderBy, this._orderDirection) :
                new Specification<T>(this.GetPredicate(), this._orderBy, this._orderDirection, this._pageSize, this._pageNumber);

            return specification;
        }

        private Expression<Func<T, bool>> GetPredicate()
        {
            Expression combinedExpression = this._filters
                .Select(e => (Expression)Expression.Invoke(e, e.Parameters.Cast<Expression>()))
                .Aggregate((acc, expr) => Expression.AndAlso(acc, expr));

            return Expression.Lambda<Func<T, bool>>(combinedExpression, this._filters[0].Parameters);
        }
    }
}
