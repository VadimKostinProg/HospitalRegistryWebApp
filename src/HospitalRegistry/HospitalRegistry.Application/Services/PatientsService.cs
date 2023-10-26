using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.ServiceContracts;
using HospitalReqistry.Domain.Entities;
using HospitalReqistry.Domain.RepositoryContracts;

namespace HospitalRegistry.Application.Services;

public class PatientsService : IPatientsService
{
    private readonly IAsyncRepository _repository;

    public PatientsService(IAsyncRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<PatientResponse>> GetAllAsync()
    {
        var patients = await _repository.GetAllAsync<Patient>();

        return patients.Select(x => x.ToPatientResponse()).ToList();
    }

    public async Task<PatientResponse> GetByIdAsync(Guid id)
    {
        var patient = await _repository.GetByIdAsync<Patient>(id);

        if (patient is null)
            throw new KeyNotFoundException("Patient with such id does not exist.");

        return patient.ToPatientResponse();
    }

    public async Task<PatientResponse> CreateAsync(PatientAddRequest request)
    {
        // Validating the request
        if (request is null)
            throw new ArgumentNullException("Patient to insert is null.");
        
        if (string.IsNullOrEmpty(request.Name))
            throw new ArgumentException("Patients name cannot be blank.");

        if (string.IsNullOrEmpty(request.Surname))
            throw new ArgumentException("Patients surname cannot be blank.");

        if (string.IsNullOrEmpty(request.Patronymic))
            throw new ArgumentException("Patients patronymic cannot be blank.");

        if (request.DateOfBirth.CompareTo(DateOnly.FromDateTime(DateTime.Now)) > 0)
            throw new ArgumentException("Incorrect patients date of birth.");
        
        // Adding patient
        var patient = request.ToPatient();
        await _repository.AddAsync(patient);

        return patient.ToPatientResponse();
    }

    public async Task<PatientResponse> UpdateAsync(PatientUpdateRequest request)
    {
        // Validating the request
        if (request is null)
            throw new ArgumentNullException("Patient to insert is null.");
        
        if (!(await _repository.ContainsAsync<Patient>(x => x.Id == request.Id)))
            throw new KeyNotFoundException("Patient with such id does not exist.");
        
        if (string.IsNullOrEmpty(request.Name))
            throw new ArgumentException("Patients name cannot be blank.");

        if (string.IsNullOrEmpty(request.Surname))
            throw new ArgumentException("Patients surname cannot be blank.");

        if (string.IsNullOrEmpty(request.Patronymic))
            throw new ArgumentException("Patients patronymic cannot be blank.");

        if (request.DateOfBirth.CompareTo(DateOnly.FromDateTime(DateTime.Now)) > 0)
            throw new ArgumentException("Incorrect patients date of birth.");
        
        // Updating the patient
        var patient = request.ToPatient();
        await _repository.UpdateAsync(patient);

        return patient.ToPatientResponse();
    }

    public async Task DeleteAsync(Guid id)
    {
        var result = await _repository.DeleteAsync<Patient>(id);

        if (!result)
            throw new KeyNotFoundException("Patient with such id does not exist.");
    }
}