using HospitalRegistry.Application.Enums;
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
            if (!this._filters.Any())
                return null;

            Expression<Func<T, bool>> combinedExpression = this._filters.First();

            if (this._filters.Count > 1)
                for (int i = 0; i < this._filters.Count - 1; i++)
                    combinedExpression = this.Combine(_filters[i], _filters[i + 1]);

            return combinedExpression;
        }

        private Expression<Func<T, bool>> Combine(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var body = Expression.AndAlso(
                    Expression.Invoke(left, param),
                    Expression.Invoke(right, param)
                );
            var lambda = Expression.Lambda<Func<T, bool>>(body, param);
            return lambda;
        }
    }
}
