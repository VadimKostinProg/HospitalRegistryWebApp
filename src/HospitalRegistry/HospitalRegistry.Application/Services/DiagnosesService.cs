using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.ServiceContracts;
using HospitalReqistry.Domain.Entities;
using HospitalReqistry.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistry.Application.Services
{
    public class DiagnosesService : IDiagnosesService
    {
        private readonly IAsyncRepository _repository;
        private readonly ISpecificationsService _specificationsService;

        public DiagnosesService(IAsyncRepository repository, ISpecificationsService specificationsService)
        {
            _repository = repository;
            _specificationsService = specificationsService;
        }

        public async Task<IEnumerable<DiagnosisResponse>> GetAllAsync(Specifications specifications)
        {
            var query = await _repository.GetAllAsync<Diagnosis>();

            if (specifications is not null)
            {
                query = _specificationsService.ApplySpecifications(query, specifications);
            }

            var diagnoses = query.Select(x => x.ToDiagnosisResponse()).ToList();

            return diagnoses;
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
            var result = await _repository.DeleteAsync<Diagnosis>(id);

            if (!result)
            {
                throw new KeyNotFoundException("Diagnosis with such id does not exist.");
            }
        }
    }
}