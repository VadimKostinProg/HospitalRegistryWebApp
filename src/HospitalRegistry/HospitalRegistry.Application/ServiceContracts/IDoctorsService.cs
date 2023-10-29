using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;

namespace HospitalRegistry.Application.ServiceContracts
{
    /// <summary>
    /// Service for managing the doctors.
    /// </summary>
    public interface IDoctorsService : ICRUDService<DoctorAddRequest, DoctorUpdateRequest, DoctorResponse>, IRecoverable
    {

        /// <summary>
        /// Method for reading filtered doctors.
        /// </summary>
        /// <param name="specifications">Specifications to filter doctors.</param>
        /// <returns>Collection IEnumerable of doctor response DTO.</returns>
        Task<IEnumerable<DoctorResponse>> GetFilteredAsync(UserSpecifications specifications);


        /// <summary>
        /// Method to get all doctors filtered by specialty.
        /// </summary>
        /// <param name="specialty">Specialty of doctor to filter.</param>
        /// <returns>Collection IEnumerable of DoctorResponse DTO.</returns>
        Task<IEnumerable<DoctorResponse>> GetBySpecialtyAsync(Specialty specialty);
    }
}
