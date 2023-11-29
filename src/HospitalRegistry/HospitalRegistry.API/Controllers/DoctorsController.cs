using HospitalRegistry.Application.Constants;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Helpers;
using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalRegistry.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorsService _doctorsService;
        private readonly ISchedulesService _schedulesService;
        private readonly IAppointmentsService _appointmentsService;

        public DoctorsController(IDoctorsService doctorsService, ISchedulesService schedulesService, IAppointmentsService appointmentsService)
        {
            _doctorsService = doctorsService;
            _schedulesService = schedulesService;
            _appointmentsService = appointmentsService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DoctorResponse>>> GetDoctorsList(
            [FromQuery] DoctorSpecificationsDTO specifications)
        {
            return Ok(await _doctorsService.GetDoctorsListAsync(specifications));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<DoctorResponse>> GetDoctorById([FromRoute] Guid id)
        {
            return Ok(await _doctorsService.GetByIdAsync(id));
        }

        [HttpPost]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<DoctorResponse>> CreateDoctor([FromBody] DoctorAddRequest request)
        {
            return Ok(await _doctorsService.CreateAsync(request));
        }

        [HttpPut]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<DoctorResponse>> UpdateDoctor([FromBody] DoctorUpdateRequest request)
        {
            return Ok(await _doctorsService.UpdateAsync(request));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<string>> DeleteDoctor([FromRoute] Guid id)
        {
            await _doctorsService.DeleteAsync(id);

            return Ok("Doctor record have been successfully deleted.");
        }

        [HttpGet("deleted")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<IEnumerable<DoctorResponse>>> GetDeletedDoctors(
            [FromQuery] DoctorSpecificationsDTO specifications)
        {
            return Ok(await _doctorsService.GetDeletedDoctorsListAsync(specifications));
        }

        [HttpPost("{id}/recover")]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<string>> RecoverDoctor([FromRoute] Guid id)
        {
            await _doctorsService.RecoverAsync(id);

            return Ok("Doctor record has been successfully recovered.");
        }

        [HttpGet("{id}/schedule")]
        [AllowAnonymous]
        public async Task<ActionResult<ScheduleDTO>> GetDoctorsSchedule([FromRoute] Guid id, [FromQuery] int? dayOfWeek)
        {
            return Ok(await _schedulesService.GetScheduleByDoctorAsync(id, dayOfWeek));
        }

        [HttpPost("schedule")]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult> SetDoctorsSchedule([FromBody] ScheduleDTO schedule)
        {
            await _schedulesService.SetAsync(schedule);

            return Ok($"Schedule for doctor \"{schedule.DoctorId}\" has been successfully set.");
        }

        [HttpGet("{id}/appointments/history")]
        [Authorize(Roles = $"{UserRoles.Doctor}, {UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetDoctorsAppointmentsHistory(
            [FromRoute] Guid id)
        {
            return Ok(await _appointmentsService.GetAppointmentsHistoryOfDoctorAsync(id));
        }

        [HttpGet("{id}/appointments/scheduled")]
        [Authorize(Roles = $"{UserRoles.Doctor}, {UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetDoctorsScheduledAppointments(
            [FromRoute] Guid id, [FromQuery] DateOnly? date)
        {
            return Ok(await _appointmentsService.GetScheduledAppoitnmentsOfDoctorAsync(id, date));
        }
    }
}