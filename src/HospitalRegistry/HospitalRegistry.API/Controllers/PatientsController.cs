using HospitalRegistry.Application.Constants;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalRegistry.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientsService _patientsService;
        private readonly IAppointmentsService _appointmentsService;

        public PatientsController(IPatientsService patientsService, IAppointmentsService appointmentsService)
        {
            _patientsService = patientsService;
            _appointmentsService = appointmentsService;
        }

        [HttpGet]
        [Authorize(Roles = $"{UserRoles.Doctor}, {UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<IEnumerable<PatientResponse>>> GetPatientsList([FromQuery] Specifications specifications)
        {
            return Ok(await _patientsService.GetAllAsync(specifications));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PatientResponse>> GetPatientById([FromRoute] Guid id)
        {
            return Ok(await _patientsService.GetByIdAsync(id));
        }

        [HttpPost]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<PatientResponse>> CreatePatient([FromBody] PatientAddRequest request)
        {
            return Ok(await _patientsService.CreateAsync(request));
        }

        [HttpPut]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<PatientResponse>> UpdatePatient([FromBody] PatientUpdateRequest request)
        {
            return Ok(await _patientsService.UpdateAsync(request));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<string>> DeletePatient([FromRoute] Guid id)
        {
            await _patientsService.DeleteAsync(id);

            return Ok($"Patient record have been successfully deleted.");
        }

        [HttpPost("{id}/recover")]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<string>> RecoverPatient([FromRoute] Guid id)
        {
            await _patientsService.RecoverAsync(id);

            return Ok($"Patient has been successfully recovered.");
        }

        [HttpGet("{id}/appointments/history")]
        [Authorize(Roles = $"{UserRoles.Patient}, {UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetPatientsAppointmentHistory(
            [FromRoute] Guid id)
        {
            return Ok(await _appointmentsService.GetAppointmentsHistoryOfPatientAsync(id));
        }

        [HttpGet("{id}/appointments/scheduled")]
        [Authorize(Roles = $"{UserRoles.Patient}, {UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetPatientsScheduledAppointments(
            [FromRoute] Guid id, [FromQuery] DateOnly date)
        {
            return Ok(await _appointmentsService.GetScheduledAppoitnmentsOfPatientAsync(id, date));
        }
    }
}