using HospitalRegistry.Application.DTO;

namespace HospitalRegistry.Application.ServiceContracts
{
    /// <summary>
    /// Service for CRUD operations.
    /// </summary>
    /// <typeparam name="TCreateRequest">DTO for creating new object.</typeparam>
    /// <typeparam name="TUpdateRequest">DTO for updating existant object.</typeparam>
    /// <typeparam name="TResponse">DTO for response.</typeparam>
    public interface ICRUDService<TCreateRequest, TUpdateRequest, TResponse>
    {
        /// <summary>
        /// Method for reading all objects.
        /// </summary>
        /// <param name="specifications">Specifications to filter, sort, search and paginate objects.</param>
        /// <returns>Collection IEnumerable of response DTO.</returns>
        Task<IEnumerable<TResponse>> GetAllAsync(Specifications specifications);

        /// <summary>
        /// Method for reading single object by its id.
        /// </summary>
        /// <param name="id">Id of object to read.</param>
        /// <returns>DTO with passed Id.</returns>
        Task<TResponse> GetByIdAsync(Guid id);

        /// <summary>
        /// Method for creating new object.
        /// </summary>
        /// <param name="request">DTO for create request.</param>
        /// <returns>Response DTO which has been created.</returns>
        Task<TResponse> CreateAsync(TCreateRequest request);

        /// <summary>
        /// Method for updating existant object.
        /// </summary>
        /// <param name="request">DTO for update request.</param>
        /// <returns>Response DTO which has been updated.</returns>
        Task<TResponse> UpdateAsync(TUpdateRequest request);

        /// <summary>
        /// Method for deleting object.
        /// </summary>
        /// <param name="id">Id of the object to delete.</param>
        Task DeleteAsync(Guid id);
    }
}
