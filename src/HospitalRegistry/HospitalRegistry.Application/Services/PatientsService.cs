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
    
    public async Task<ListModel<PatientResponse>> GetPatientsListAsync(PatientSpecificationsDTO specificationsDTO)
    {
        if (specificationsDTO is null)
            throw new ArgumentNullException("Specifications are null.");

        return await this.GetPatientsListBySpecificationAsync(this.GetSpecification(specificationsDTO));
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

            if (!string.IsNullOrEmpty(specificationsDTO.SortField))
            {
                switch (specificationsDTO.SortField)
                {
                    case "Id":
                        builder.OrderBy(x => x.Id, specificationsDTO.SortDirection ?? Enums.SortDirection.ASC);
                        break;
                    case "Name":
                        builder.OrderBy(x => x.Name, specificationsDTO.SortDirection ?? Enums.SortDirection.ASC);
                        break;
                    case "Surname":
                        builder.OrderBy(x => x.Surname, specificationsDTO.SortDirection ?? Enums.SortDirection.ASC);
                        break;
                    case "Patronymic":
                        builder.OrderBy(x => x.Patronymic, specificationsDTO.SortDirection ?? Enums.SortDirection.ASC);
                        break;
                    case "DateOfBirth":
                        builder.OrderBy(x => x.DateOfBirth, specificationsDTO.SortDirection ?? Enums.SortDirection.ASC);
                        break;
                }
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

    public async Task<ListModel<PatientResponse>> GetDeletedPatientsListAsync(PatientSpecificationsDTO specificationsDTO)
    {
        if (specificationsDTO is null)
            throw new ArgumentNullException("Specifications are null.");

        return await this.GetPatientsListBySpecificationAsync(this.GetSpecification(specificationsDTO, isDeleted: true));
    }

    private async Task<ListModel<PatientResponse>> GetPatientsListBySpecificationAsync(ISpecification<Patient> specification)
    {
        var patients =
            (await _repository.GetAsync<Patient>(specification, disableTracking: false))
            .Select(x => x.ToPatientResponse())
            .ToList();

        var totalCount = specification.Predicate is null ?
            await _repository.CountAsync<Patient>() :
            await _repository.CountAsync<Patient>(specification.Predicate);

        var totalPages = (int)Math.Ceiling((double)totalCount / specification.Take);

        return new ListModel<PatientResponse>
        {
            List = patients,
            TotalPages = totalPages
        };
    }
}