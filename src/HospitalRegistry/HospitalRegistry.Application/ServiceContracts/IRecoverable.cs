namespace HospitalRegistry.Application.ServiceContracts
{
    /// <summary>
    /// Tools for recovering functional.
    /// </summary>
    /// <typeparam name="TResponse">DTO for response.</typeparam>
    /// <typeparam name="TSpecifications">DTO for specifications.</typeparam>
    public interface IRecoverable
    {
        /// <summary>
        /// Method to recover deleted entity in the data source.
        /// </summary>
        /// <param name="id">Id of the entity to recover.</param>
        Task RecoverAsync(Guid id);
    }
}
