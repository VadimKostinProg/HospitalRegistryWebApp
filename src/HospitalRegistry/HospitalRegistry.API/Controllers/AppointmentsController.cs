using HospitalRegistry.Application.Constants;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalRegistry.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentsService _appointmentsService;

        public AppointmentsController(IAppointmentsService appointmentsService)
        {
            _appointmentsService = appointmentsService;
        }

        [HttpGet]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetAppointmentsList(
            [FromQuery] AppointmentSpecificationsDTO specifications)
        {
            return Ok(await _appointmentsService.GetAppointmnetsListAsync(specifications));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentResponse>> GetAppointmentById([FromRoute] Guid id)
        {
            return Ok(await _appointmentsService.GetAppointmentByIdAsync(id));
        }

        [HttpGet("free-slots")]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<IEnumerable<AppointmentSlotResponse>>> SearchFreeSlots(
            [FromQuery] FreeSlotsSearchSpecifications specifications)
        {
            return Ok(await _appointmentsService.SearchFreeSlotsAsync(specifications));
        }

        [HttpPost]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<string>> SetAppointment([FromBody] AppointmentSetRequest request)
        {
            await _appointmentsService.SetAppointmentAsync(request);

            return Ok("Appointment has been successfully set.");
        }

        [HttpPut]
        [Authorize(Roles = UserRoles.Doctor)]
        public async Task<ActionResult<string>> CompleteAppointment([FromBody] AppointmentCompleteRequest request)
        {
            await _appointmentsService.CompleteAppointmentAsync(request);

            return Ok("Appointment has been successfully completed.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<string>> CancelAppointment([FromRoute] Guid id)
        {
            await _appointmentsService.CancelAppointmentAsync(id);

            return Ok($"Appointment \"{id}\" has been successfully canceled.");
        }

        [HttpPost("{id}/recover")]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<string>> RecoverAppointment([FromRoute] Guid id)
        {
            await _appointmentsService.RecoverAsync(id);

            return Ok($"Appointment \"{id}\" has been successfully recovered.");
        }

        [HttpDelete("canceled")]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<string>> ClearAllCanceledAppointments()
        {
            await _appointmentsService.ClearAllCanceledAppointmentsAsync();

            return Ok("All canceled appointments has been successfully cleared.");
        }
    }
}
