using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using HospitalRegistry.Application.ServiceContracts;
using HospitalReqistry.Domain.RepositoryContracts;

namespace HospitalRegistry.Application.Services
{
    public class DoctorsService : IDoctorsService
    {
        private readonly IAsyncRepository _repository;

        public DoctorsService(IAsyncRepository repository)
        {
            _repository = repository;
        }

        public Task<DoctorResponse> CreateAsync(DoctorAddRequest request)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DoctorResponse>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DoctorResponse> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DoctorResponse>> GetBySpecialtyAsync(Specialty specialty)
        {
            throw new NotImplementedException();
        }

        public Task<DoctorResponse> UpdateAsync(DoctorUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
