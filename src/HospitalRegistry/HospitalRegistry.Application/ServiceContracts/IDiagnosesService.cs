using HospitalRegistry.Application.DTO;

namespace HospitalRegistry.Application.ServiceContracts
{
    /// <summary>
    /// Service for managing the diagnoses.
    /// </summary>
    public interface IDiagnosesService : 
        ICRUDService<DiagnosisCreateRequest, DiagnosisUpdateRequest, DiagnosisResponse>, 
        IRecoverable
    {
        /// <summary>
        /// Method for read the diagnosis list with applyed specifications.
        /// </summary>
        /// <param name="specificationsDTO">Specifications for filtering, sorting and pagination to apply.</param>
        /// <returns>List of diagnoses with applyed specifications.</returns>
        Task<ListModel<DiagnosisResponse>> GetDiagnosesListAsync(DiagnosisSpecificationsDTO specificationsDTO);

        /// <summary>
        /// Method for reading all deleted diagnoses.
        /// </summary>
        /// <param name="specificationsDTO">Specifications for filtering, sorting and paginating to apply.</param>
        /// <returns>List of deleted diagnoses with applyed specifications.</returns>
        Task<ListModel<DiagnosisResponse>> GetDeletedDiagnosesListAsync(DiagnosisSpecificationsDTO specificationsDTO);
    }
}
