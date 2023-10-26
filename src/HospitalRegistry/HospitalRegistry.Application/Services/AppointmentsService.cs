using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.ServiceContracts;
using HospitalReqistry.Domain.RepositoryContracts;

namespace HospitalRegistry.Application.Services;

public class AppointmentsService : IAppointmentsService
{
    private readonly IAsyncRepository _repository;
    private readonly ISchedulesService _schedulesService;

    public AppointmentsService(IAsyncRepository repository, ISchedulesService schedulesService)
    {
        _repository = repository;
        _schedulesService = schedulesService;
    }

    public Task<IEnumerable<FreeSlotResponse>> SearchFreeSlotsAsync(FreeSlotsSearchSpecifications specifications)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AppointmentResponse>> GetAppointmentsHistoryOfPatient(Guid patientId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AppointmentResponse>> GetAppointmentsHistoryOfDoctor(Guid doctorId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AppointmentResponse>> GetScheduledAppoitnmentsOfPatient(Guid patientId, DateOnly date)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AppointmentResponse>> GetScheduledAppoitnmentsOfDoctor(Guid doctorId, DateOnly date)
    {
        throw new NotImplementedException();
    }

    public Task SetAppointmentAsync(AppointmentSetRequest request)
    {
        throw new NotImplementedException();
    }

    public Task CompleteAppointmentAsync(AppointmentCompleteRequest request)
    {
        throw new NotImplementedException();
    }

    public Task CancelAppointmentAsync(Guid appointmentId)
    {
        throw new NotImplementedException();
    }

    public Task RecoverAppointmentAsync(Guid appointmentId)
    {
        throw new NotImplementedException();
    }

    public Task ClearAllCanceledAppointmentsAsync()
    {
        throw new NotImplementedException();
    }
}