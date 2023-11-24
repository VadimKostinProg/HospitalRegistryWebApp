using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using System.ComponentModel;
using System.Linq.Expressions;

namespace HospitalRegistry.Application.Services
{
    public class SpecificationsService : SpecificationsServiceBase
    {
        protected override IQueryable<T> ApplyFiltering<T>(IQueryable<T> query, Specifications specifications)
        {
            if (specifications.Filters is null || !specifications.Filters.Any())
            {
                return query;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var predicate = GetPredicate<T>(parameter, specifications.Filters);

            return query.Where(predicate);
        }

        protected override IQueryable<T> ApplyPagination<T>(IQueryable<T> query, Specifications specifications)
        {
            return query.Skip((specifications.PageNumber - 1) * specifications.PageSize).Take(specifications.PageSize);
        }

        protected override IQueryable<T> ApplySearching<T>(IQueryable<T> query, Specifications specifications)
        {
            if (string.IsNullOrEmpty(specifications.SearchTerm) || typeof(T).GetProperty("Name") is null)
            {
                return query;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var searchTermToLower = specifications.SearchTerm.ToLower();
            var predicate = GetPredicate<T>(parameter, searchTermToLower);

            return query.Where(predicate);
        }

        protected override IQueryable<T> ApplySorting<T>(IQueryable<T> query, Specifications specifications)
        {
            if (string.IsNullOrEmpty(specifications.SortField))
            {
                return query;
            }

            var entityType = typeof(T);
            var sortedProperty = entityType.GetProperty(specifications.SortField);

            if (sortedProperty is null)
            {
                throw new ArgumentNullException($"Property of name {specifications.SortField} does not exist in type {entityType.Name}");
            }

            var parameterExpression = Expression.Parameter(entityType, "x");
            var propertyExpression = Expression.Property(parameterExpression, sortedProperty);
            var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(propertyExpression, typeof(object)), parameterExpression);

            return specifications.SortDirection == SortDirection.ASC
                ? query.OrderBy(lambda).AsQueryable()
                : query.OrderByDescending(lambda).AsQueryable();
        }

        public Expression<Func<T, bool>> GetPredicate<T>(ParameterExpression parameter, IDictionary<string, string> filters)
        {
            var conditions = filters.Select(filter =>
            {
                var property = Expression.Property(parameter, filter.Key);

                var keyType = typeof(T).GetProperty(filter.Key).PropertyType;
                var value = TypeDescriptor.GetConverter(keyType).ConvertFromInvariantString(filter.Value);
                var constant = Expression.Constant(value);
                var equality = Expression.Equal(property, constant);

                return equality;
            });

            var andExpressions = conditions.Aggregate(Expression.AndAlso);

            return Expression.Lambda<Func<T, bool>>(andExpressions, parameter);
        }

        // Get predicate for searching
        private Expression<Func<T, bool>> GetPredicate<T>(ParameterExpression parameter, string searchTerm)
        {
            var searchableProperties = new[]
            {
                "Name",
                "Surname",
                "Patronymic"
            };

            // Filtering only such searchable properties that are present in the type
            searchableProperties = searchableProperties
                .Where(prop => typeof(T).GetProperty(prop) != null)
                .ToArray();

            var orExpressions = searchableProperties
                .Select(propertyName => GetPropertyContainsExpression<T>(parameter, propertyName, searchTerm))
                .Aggregate(Expression.OrElse);

            return Expression.Lambda<Func<T, bool>>(orExpressions, parameter);
        }

        private Expression GetPropertyContainsExpression<T>(
            ParameterExpression parameter, string propertyName, string searchTerm)
        {
            var propertyExpression = Expression.Property(parameter, propertyName);
            var toStringExpression = Expression.Call(propertyExpression, "ToString", null);
            var toLowerExpression = Expression.Call(toStringExpression, "ToLower", null);
            return Expression.Call(toLowerExpression, "Contains", null, Expression.Constant(searchTerm));
        }
    }
}
