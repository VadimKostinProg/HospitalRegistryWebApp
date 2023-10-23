using HospitalReqistry.Domain.Entities;
using System.Linq.Expressions;

namespace HospitalReqistry.Domain.RepositoryContracts
{
    /// <summary>
    /// Data access repository for EntityBase objects.
    /// </summary>
    public interface IAsyncRepository
    {
        /// <summary>
        /// Method for reading all entities of the specific type from the data source.
        /// </summary>
        /// <typeparam name="T">Type of entities to read.</typeparam>
        /// <param name="disableTracking">Flag for enabling a tracking.</param>
        /// <returns>Collection IEnumerable of entities.</returns>
        Task<IEnumerable<T>> GetAllAsync<T>(bool disableTracking = true) where T : EntityBase;

        /// <summary>
        /// Method for reading entities of the specific type from the data source filtered by predicate.
        /// </summary>
        /// <typeparam name="T">Type of entities to read.</typeparam>
        /// <param name="predicate">Expression to filter entities.</param>
        /// <param name="disableTracking">Flag for enabling a tracking.</param>
        /// <returns>Collection IEnumerable of entities.</returns>
        Task<IEnumerable<T>> GetFilteredAsync<T>(Expression<Func<T, bool>> predicate,
                                                 bool disableTracking = true) 
                                                 where T : EntityBase;

        /// <summary>
        /// Method for searching the entity of the specific type from the data source by its id.
        /// </summary>
        /// <typeparam name="T">Type of entity to search.</typeparam>
        /// <param name="id">Guid of the entity.</param>
        /// <returns>Found entity of the specific type, null - if entity with passed id was not found.</returns>
        Task<T?> GetByIdAsync<T>(Guid id) where T : EntityBase;

        /// <summary>
        /// Method for searching the entity of the specific type from the data source by specific criteria.
        /// </summary>
        /// <typeparam name="T">Type of entity to search.</typeparam>
        /// <param name="expression">Expression to search.</param>
        /// <returns>Found entity of the specific type, null - if any entity with passed criteria was not found.</returns>
        Task<T?> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Method for checking if the data source contains the entity.
        /// </summary>
        /// <typeparam name="T">Type of entity to check.</typeparam>
        /// <param name="expression">Expression to check.</param>
        /// <returns>True - if the data source contains the entity, otherwise - false.</returns>
        Task<bool> ContainsAsync<T>(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Method for inserting new entity into the data source.
        /// </summary>
        /// <typeparam name="T">Type of entity to insert.</typeparam>
        /// <param name="entity">Entity to insert.</param>
        Task AddAsync<T>(T entity) where T : EntityBase;

        /// <summary>
        /// Method for updating existent entity in the data source.
        /// </summary>
        /// <typeparam name="T">Type of entity to update.</typeparam>
        /// <param name="entity">Entity to update.</param>
        Task UpdateAsync<T>(T entity) where T : EntityBase;

        /// <summary>
        /// Method for deleting existent entity form the data source.
        /// </summary>
        /// <typeparam name="T">Type of entity to delete.</typeparam>
        /// <param name="id">Guid of entity to delete.</param>
        /// <returns>True - if deleting was successfull, otherwise - false.</returns>
        Task<bool> DeleteAsync<T>(Guid id) where T : EntityBase;
    }
}
