using HospitalRegistry.Application.Constants;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalRegistry.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
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
            [FromQuery] Specifications specifications)
        {
            return Ok(await _doctorsService.GetAllAsync(specifications));
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

            return Ok("Doctor record and account have been successfully deleted.");
        }

        [HttpPost("recover/{id}")]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<string>> RecoverDoctor([FromRoute] Guid id)
        {
            await _doctorsService.RecoverAsync(id);

            return Ok("Doctor record has been successfully recovered.");
        }

        [HttpGet("{id}/schedule")]
        [AllowAnonymous]
        public async Task<ActionResult<ScheduleDTO>> GetDoctorsSchedule([FromRoute] Guid id)
        {
            return Ok(await _schedulesService.GetScheduleByDoctorAsync(id));
        }

        [HttpPost("schedule")]
        [HttpGet]
        [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult> SetDoctorsSchedule([FromBody] ScheduleDTO schedule)
        {
            await _schedulesService.SetAsync(schedule);

            return Ok($"Schedule for doctor \"{schedule.DoctorId}\" has been successfully set.");
        }

        [HttpGet("{id}/appointmnets/history")]
        [HttpGet]
        [Authorize(Roles = $"{UserRoles.Doctor}, {UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetDoctorsAppointmentsHistory(
            [FromRoute] Guid id)
        {
            return Ok(await _appointmentsService.GetAppointmentsHistoryOfDoctorAsync(id));
        }

        [HttpGet("{id}/appointments/scheduled")]
        [Authorize(Roles = $"{UserRoles.Doctor}, {UserRoles.Admin}, {UserRoles.Receptionist}")]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetDoctorsScheduledAppointments(
            [FromRoute] Guid id, [FromQuery] DateOnly date)
        {
            return Ok(await _appointmentsService.GetScheduledAppoitnmentsOfDoctorAsync(id, date));
        }
    }
}