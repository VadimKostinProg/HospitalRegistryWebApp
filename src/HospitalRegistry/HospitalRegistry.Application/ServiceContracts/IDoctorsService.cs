using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;

namespace HospitalRegistry.Application.ServiceContracts
{
    /// <summary>
    /// Service for managing the doctors.
    /// </summary>
    public interface IDoctorsService : ICRUDService<DoctorAddRequest, DoctorUpdateRequest, DoctorResponse>, 
        IRecoverable
    {
        /// <summary>
        /// Method for read the doctors list with applyed specifications.
        /// </summary>
        /// <param name="specificationsDTO">Specifications for filtering, sorting and pagination to apply.</param>
        /// <returns>Collection IEnumerable of doctors with applyed specifications.</returns>
        Task<IEnumerable<DoctorResponse>> GetDoctorsListAsync(DoctorSpecificationsDTO specificationsDTO);

        /// <summary>
        /// Method to get all doctors filtered by specialty.
        /// </summary>
        /// <param name="specialty">Specialty of doctor to filter.</param>
        /// <returns>Collection IEnumerable of DoctorResponse DTO.</returns>
        Task<IEnumerable<DoctorResponse>> GetBySpecialtyAsync(Specialty specialty);

        /// <summary>
        /// Method for reading all deleted doctors.
        /// </summary>
        /// <param name="specificationsDTO">Specifications for filtering, sorting and paginating to apply.</param>
        /// <returns>Collection IEnumerable of deleted doctors with applyed specifications.</returns>
        Task<IEnumerable<DoctorResponse>> GetDeletedDoctorsListAsync(DoctorSpecificationsDTO specificationsDTO);
    }
}
