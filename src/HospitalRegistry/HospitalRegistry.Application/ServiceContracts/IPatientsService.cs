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
        /// Method for read the patients list with applyed specifications.
        /// </summary>
        /// <param name="specificationsDTO">Specifications for filtering, sorting and pagination to apply.</param>
        /// <returns>Collection IEnumerable of patients with applyed specifications.</returns>
        Task<IEnumerable<PatientResponse>> GetPatientsListAsync(PatientSpecificationsDTO specificationsDTO);
    }
}
