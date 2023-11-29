using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Application.RepositoryContracts;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.Services;

public class PatientsService : IPatientsService
{
    private readonly IAsyncRepository _repository;

    public PatientsService(IAsyncRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<PatientResponse>> GetPatientsListAsync(PatientSpecificationsDTO specificationsDTO)
    {
        var patients = (await _repository.GetAsync<Patient>(this.GetSpecification(specificationsDTO)))
            .Select(x => x.ToPatientResponse())
            .ToList();

        return patients;
    }

    private ISpecification<Patient> GetSpecification(PatientSpecificationsDTO specificationsDTO, bool isDeleted = false)
    {
        var builder = new SpecificationBuilder<Patient>();

        builder.With(x => x.IsDeleted == isDeleted);

        if (specificationsDTO is not null)
        {
            if (!string.IsNullOrEmpty(specificationsDTO.Name))
                builder.With(x => x.Name == specificationsDTO.Name);

            if (!string.IsNullOrEmpty(specificationsDTO.Surname))
                builder.With(x => x.Surname == specificationsDTO.Surname);

            if (!string.IsNullOrEmpty(specificationsDTO.Patronymic))
                builder.With(x => x.Patronymic == specificationsDTO.Patronymic);

            if (specificationsDTO.DateOfBirth is not null)
                builder.With(x => x.DateOfBirth == specificationsDTO.DateOfBirth.Value.ToString());

            switch (specificationsDTO.SortField)
            {
                case "Id":
                    builder.OrderBy(x => x.Id, specificationsDTO.SortDirection);
                    break;
                case "Name":
                    builder.OrderBy(x => x.Name, specificationsDTO.SortDirection);
                    break;
                case "Surname":
                    builder.OrderBy(x => x.Surname, specificationsDTO.SortDirection);
                    break;
                case "Patronymic":
                    builder.OrderBy(x => x.Patronymic, specificationsDTO.SortDirection);
                    break;
                case "DateOfBirth":
                    builder.OrderBy(x => x.DateOfBirth, specificationsDTO.SortDirection);
                    break;
            }

            builder.WithPagination(specificationsDTO.PageSize, specificationsDTO.PageNumber);
        }

        return builder.Build();
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
        var patient = await _repository.GetByIdAsync<Patient>(id);

        if (patient is null)
            throw new KeyNotFoundException("Patient with such id does not exist.");

        if (patient.IsDeleted)
            throw new ArgumentException("Patient is already deleted.");

        patient.IsDeleted = true;
        await _repository.UpdateAsync(patient);
    }

    public async Task RecoverAsync(Guid id)
    {
        var patient = await _repository.GetByIdAsync<Patient>(id);

        if (patient is null)
            throw new KeyNotFoundException("Patient with such id does not exist.");

        if (!patient.IsDeleted)
            throw new ArgumentException("Patient has been not deleted for recovering.");

        patient.IsDeleted = false;
        await _repository.UpdateAsync(patient);
    }

    public async Task<IEnumerable<PatientResponse>> GetDeletedPatientsListAsync(PatientSpecificationsDTO specificationsDTO)
    {
        var patients = 
            (await _repository.GetAsync<Patient>(this.GetSpecification(specificationsDTO, isDeleted: true)))
            .Select(x => x.ToPatientResponse())
            .ToList();

        return patients;
    }
}