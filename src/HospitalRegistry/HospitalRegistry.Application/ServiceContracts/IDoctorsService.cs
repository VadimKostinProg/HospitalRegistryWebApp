using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;

namespace HospitalRegistry.Application.ServiceContracts
{
    /// <summary>
    /// Service for managing the doctors.
    /// </summary>
    public interface IDoctorsService : ICRUDService<DoctorAddRequest, DoctorUpdateRequest, DoctorResponse>
    {
        /// <summary>
        /// Method to get all doctors filtered by specialty.
        /// </summary>
        /// <param name="specialty">Specialty of doctor to filter.</param>
        /// <returns>Collection IEnumerable of DoctorResponse DTO.</returns>
        Task<IEnumerable<DoctorResponse>> GetBySpecialtyAsync(Specialty specialty);
    }
}
