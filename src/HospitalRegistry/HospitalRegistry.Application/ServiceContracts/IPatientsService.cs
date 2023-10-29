using HospitalRegistry.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalRegistry.Application.ServiceContracts
{
    /// <summary>
    /// Service for managing patients.
    /// </summary>
    public interface IPatientsService : ICRUDService<PatientAddRequest, PatientUpdateRequest, PatientResponse>, IRecoverable
    {
        /// <summary>
        /// Method for reading filtered patients.
        /// </summary>
        /// <param name="specifications">Specifications to filter patients.</param>
        /// <returns>Collection IEnumerable of patient response DTO.</returns>
        Task<IEnumerable<PatientResponse>> GetFilteredAsync(UserSpecifications specifications);
    }
}
