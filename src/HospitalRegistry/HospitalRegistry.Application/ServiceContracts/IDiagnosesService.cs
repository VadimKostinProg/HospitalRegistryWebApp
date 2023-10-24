using HospitalRegistry.Application.DTO;

namespace HospitalRegistry.Application.ServiceContracts
{
    /// <summary>
    /// Service for managing the diagnoses.
    /// </summary>
    public interface IDiagnosesService : ICRUDService<DiagnosisAddRequest, DiagnosisUpdateRequest, DiagnosisResponse>
    {
    }
}
