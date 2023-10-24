using HospitalRegistry.Application.DTO;

namespace HospitalRegistry.Application.ServiceContracts
{
    /// <summary>
    /// Service for managing doctors schedules.
    /// </summary>
    public interface ISchedulesService
    {
        /// <summary>
        /// Method for reading schedule of passed doctor.
        /// </summary>
        /// <param name="doctorId">Doctors id to read schedule of.</param>
        /// <returns>Schedule DTO of passed doctor.</returns>
        Task<ScheduleDTO> GetScheduleByDoctorAsync(Guid doctorId);

        /// <summary>
        /// Method for setting new schedule to the doctor.
        /// </summary>
        /// <param name="schedule">Schedule DTO with inforamtion about the doctor and work slots.</param>
        Task SetAsync(ScheduleDTO schedule);
    }
}
