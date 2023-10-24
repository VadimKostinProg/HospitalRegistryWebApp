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
    public interface IPatientsService : ICRUDService<PatientAddRequest, PatientUpdateRequest, PatientResponse>
    {
    }
}
