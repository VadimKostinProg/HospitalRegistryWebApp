using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Application.RepositoryContracts;
using HospitalReqistry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistry.Application.Services
{
    public class DiagnosesService : IDiagnosesService
    {
        private readonly IAsyncRepository _repository;

        public DiagnosesService(IAsyncRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DiagnosisResponse>> GetDiagnosesListAsync(DiagnosisSpecificationsDTO specificationsDTO)
        {
            var diagnoses = (await _repository.GetAsync<Diagnosis>(this.GetSpecification(specificationsDTO)))
                .Select(x => x.ToDiagnosisResponse())
                .ToList();

            return diagnoses;
        }

        private ISpecification<Diagnosis> GetSpecification(DiagnosisSpecificationsDTO specifications)
        {
            var builder = new SpecificationBuilder<Diagnosis>();

            if (specifications is not null)
            {
                if (!string.IsNullOrEmpty(specifications.Name))
                    builder.With(x => x.Name.Contains(specifications.Name));

                switch (specifications.SortField)
                {
                    case "Id":
                        builder.OrderBy(x => x.Id, specifications.SortDirection);
                        break;
                    case "Name":
                        builder.OrderBy(x => x.Name, specifications.SortDirection);
                        break;
                }

                builder.WithPagination(specifications.PageSize, specifications.PageNumber);
            }

            return builder.Build();
        }

        public async Task<DiagnosisResponse> GetByIdAsync(Guid id)
        {
            var diagnosis = await _repository.GetByIdAsync<Diagnosis>(id);

            if (diagnosis is null)
            {
                throw new KeyNotFoundException("Diagnosis with such id does not exist.");
            }

            return diagnosis.ToDiagnosisResponse();
        }

        public async Task<DiagnosisResponse> CreateAsync(DiagnosisAddRequest request)
        {
            // Validating request
            if (request is null)
                throw new ArgumentNullException("Diagnosis to insert is null.");

            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException("Diagnosis name cannot be blank.");

            // Adding new diagnosis
            var diagnosis = request.ToDiagnosis();
            await _repository.AddAsync(diagnosis);

            return diagnosis.ToDiagnosisResponse();
        }

        public async Task<DiagnosisResponse> UpdateAsync(DiagnosisUpdateRequest request)
        {
            // Validating request
            if (request is null)
                throw new ArgumentNullException("Diagnosis to update is null.");

            if (!(await _repository.ContainsAsync<Diagnosis>(x => x.Id == request.Id)))
                throw new KeyNotFoundException("Diagnosis with such id does not exist.");

            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException("Diagnosis name cannot be blank.");

            // Updating the diagnosis
            var diagnosis = request.ToDiagnosis();
            await _repository.UpdateAsync(diagnosis);

            return diagnosis.ToDiagnosisResponse();
        }

        public async Task DeleteAsync(Guid id)
        {
            var diagnosis = await _repository.GetByIdAsync<Diagnosis>(id);

            if (diagnosis is null)
            {
                throw new KeyNotFoundException("Diagnosis with such id does not exist.");
            }

            if (diagnosis.IsDeleted)
            {
                throw new ArgumentException("Diagnosis is already deleted.");
            }

            diagnosis.IsDeleted = true;

            await _repository.UpdateAsync<Diagnosis>(diagnosis);
        }

        public async Task RecoverAsync(Guid id)
        {
            var diagnosis = await _repository.GetByIdAsync<Diagnosis>(id);

            if (diagnosis is null)
            {
                throw new KeyNotFoundException("Diagnosis with such id does not exist.");
            }

            if (!diagnosis.IsDeleted)
            {
                throw new ArgumentException("Diagnosis is not deleted.");
            }

            diagnosis.IsDeleted = false;

            await _repository.UpdateAsync<Diagnosis>(diagnosis);
        }
    }
}