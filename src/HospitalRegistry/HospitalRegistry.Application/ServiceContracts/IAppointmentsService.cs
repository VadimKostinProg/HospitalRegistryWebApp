using HospitalRegistry.Application.DTO;

namespace HospitalRegistry.Application.ServiceContracts
{
    /// <summary>
    /// Service for managing the appointments.
    /// </summary>
    public interface IAppointmentsService : IRecoverable
    {
        /// <summary>
        /// Method for searching the free slots for appointments.
        /// </summary>
        /// <param name="specifications">Filter specifications to search free slots.</param>
        /// <returns>Collection IEnumerable of found free slots DTO.</returns>
        Task<IEnumerable<AppointmentSlotResponse>> SearchFreeSlotsAsync(FreeSlotsSearchSpecifications specifications);

        /// <summary>
        /// Method for reading the complited appointments history of the patient.
        /// </summary>
        /// <param name="patientId">Id of the patient to read history.</param>
        /// <returns>Collection IENumerable of appointment DTO.</returns>
        Task<IEnumerable<AppointmentResponse>> GetAppointmentsHistoryOfPatientAsync(Guid patientId);

        /// <summary>
        /// Method for reading the complited appointments history of the doctor.
        /// </summary>
        /// <param name="doctorId">Id of the doctor to read history.</param>
        /// <returns>Collection IENumerable of appointment DTO.</returns>
        Task<IEnumerable<AppointmentResponse>> GetAppointmentsHistoryOfDoctorAsync(Guid doctorId);

        /// <summary>
        /// Method for reading scheduled appointments of the person on the specific date.
        /// </summary>
        /// <param name="patientId">Id of person to read scheduled appointments.</param>
        /// <param name="date">Date to read scheduled appointments on.</param>
        /// <returns>Collection IENumerable of appointment DTO.</returns>
        Task<IEnumerable<AppointmentResponse>> GetScheduledAppoitnmentsOfPatientAsync(Guid patientId, DateOnly date);

        /// <summary>
        /// Method for reading scheduled appointments of the doctor on the specific date.
        /// </summary>
        /// <param name="doctorId">Id of doctor to read scheduled appointments.</param>
        /// <param name="date">Date to read scheduled appointments on.</param>
        /// <returns>Collection IENumerable of appointment DTO.</returns>
        Task<IEnumerable<AppointmentResponse>> GetScheduledAppoitnmentsOfDoctorAsync(Guid doctorId, DateOnly date);
        
        /// <summary>
        /// Method for setting new appointment.
        /// </summary>
        /// <param name="request">Appointment to set.</param>
        Task SetAppointmentAsync(AppointmentSetRequest request);

        /// <summary>
        /// Method for completing the scheduled appointment.
        /// </summary>
        /// <param name="request">DTO with information to complete appointent.</param>
        Task CompleteAppointmentAsync(AppointmentCompleteRequest request);

        /// <summary>
        /// Method for canceling the scheduled appointment.
        /// </summary>
        /// <param name="appointmentId">Id of the appointment to cancel.</param>
        Task CancelAppointmentAsync(Guid appointmentId);

        /// <summary>
        /// Method for clearing all canceled appointments.
        /// </summary>
        Task ClearAllCanceledAppointmentsAsync();
    }
}
