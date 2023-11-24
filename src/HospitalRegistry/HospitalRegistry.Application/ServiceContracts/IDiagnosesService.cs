using HospitalRegistry.Application.DTO;

namespace HospitalRegistry.Application.ServiceContracts
{
    /// <summary>
    /// Service for managing the diagnoses.
    /// </summary>
    public interface IDiagnosesService : 
        ICRUDService<DiagnosisAddRequest, DiagnosisUpdateRequest, DiagnosisResponse>, 
        IRecoverable
    {
        /// <summary>
        /// Method for read the diagnosis list with applyed specifications.
        /// </summary>
        /// <param name="specificationsDTO">Specifications for filtering, sorting and pagination to apply.</param>
        /// <returns>Collection IEnumerable of diagnoses with applyed specifications.</returns>
        Task<IEnumerable<DiagnosisResponse>> GetDiagnosesListAsync(DiagnosisSpecificationsDTO specificationsDTO);
    }
}
