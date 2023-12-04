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
    public interface IPatientsService : ICRUDService<PatientAddRequest, PatientUpdateRequest, PatientResponse>, 
        IRecoverable
    {
        /// <summary>
        /// Method for read the patients list with applyed specifications.
        /// </summary>
        /// <param name="specificationsDTO">Specifications for filtering, sorting and pagination to apply.</param>
        /// <returns>List of patients with applyed specifications.</returns>
        Task<ListModel<PatientResponse>> GetPatientsListAsync(PatientSpecificationsDTO specificationsDTO);

        /// <summary>
        /// Method for reading all deleted patients.
        /// </summary>
        /// <param name="specificationsDTO">Specifications for filtering, sorting and paginating to apply.</param>
        /// <returns>List of deleted patients with applyed specifications.</returns>
        Task<ListModel<PatientResponse>> GetDeletedPatientsListAsync(PatientSpecificationsDTO specificationsDTO);
    }
}
