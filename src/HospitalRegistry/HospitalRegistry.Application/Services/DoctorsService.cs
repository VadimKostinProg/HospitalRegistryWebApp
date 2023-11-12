using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using HospitalRegistry.Application.ServiceContracts;
using HospitalReqistry.Domain.Entities;
using HospitalReqistry.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistry.Application.Services
{
    public class DoctorsService : IDoctorsService
    {
        private readonly IAsyncRepository _repository;
        private readonly ISpecificationsService _specificationsService;

        public DoctorsService(IAsyncRepository repository, ISpecificationsService specificationsService)
        {
            _repository = repository;
            _specificationsService = specificationsService;
        }

        public async Task<DoctorResponse> CreateAsync(DoctorAddRequest request)
        {
            // Validating the request
            if (request is null) 
                throw new ArgumentNullException("Doctor to insert is null.");

            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException("Doctors name cannot be blank.");

            if (string.IsNullOrEmpty(request.Surname))
                throw new ArgumentException("Doctors surname cannot be blank.");

            if (string.IsNullOrEmpty(request.Patronymic))
                throw new ArgumentException("Doctors patronymic cannot be blank.");

            if (request.DateOfBirth.CompareTo(DateOnly.FromDateTime(DateTime.Now.AddYears(-18))) > 0)
                throw new ArgumentException("Doctor must be at least 18 years old.");

            // Adding new doctor
            var doctor = request.ToDoctor();
            await _repository.AddAsync(doctor);

            return doctor.ToDoctorResponse();
        }

        public async Task DeleteAsync(Guid id)
        {
            var doctor = await _repository.GetByIdAsync<Doctor>(id);

            if (doctor is null)
                throw new KeyNotFoundException("Doctor with such id does not exists.");

            if (!doctor.IsDeleted)
                throw new ArgumentException("Doctor is already deleted.");

            // Deleting doctors schedule
            var scheduleSlots = doctor.Schedules;
            await _repository.DeleteRangeAsync(scheduleSlots);

            // Firing the doctor
            doctor.IsDeleted = true;
            await _repository.UpdateAsync(doctor);
        }

        public async Task RecoverAsync(Guid doctorId)
        {
            var doctor = await _repository.GetByIdAsync<Doctor>(doctorId);

            if (doctor is null)
                throw new KeyNotFoundException("Doctor with such id does not exists.");

            if (!doctor.IsDeleted)
                throw new ArgumentException("Doctor has been not deleted for recovering.");

            doctor.IsDeleted = false;
            await _repository.UpdateAsync(doctor);
        }

        public async Task<IEnumerable<DoctorResponse>> GetAllAsync(Specifications specifications)
        {
            var query = await _repository.GetAllAsync<Doctor>();

            if (specifications is not null)
            {
                query = _specificationsService.ApplySpecifications(query, specifications);
            }

            var doctors = query.Select(x => x.ToDoctorResponse()).ToList();

            return doctors;
        }

        public async Task<DoctorResponse> GetByIdAsync(Guid id)
        {
            var doctor = await _repository.GetByIdAsync<Doctor>(id);

            if (doctor is null)
                throw new KeyNotFoundException("Doctor with such id does not exists.");

            return doctor.ToDoctorResponse();
        }

        public async Task<IEnumerable<DoctorResponse>> GetBySpecialtyAsync(Specialty specialty)
        {
            var doctors = await _repository.GetFilteredAsync<Doctor>(x => x.Specialty == specialty.ToString());

            return doctors.Select(x => x.ToDoctorResponse()).ToList();
        }

        public async Task<DoctorResponse> UpdateAsync(DoctorUpdateRequest request)
        {
            // Validating the request
            if (request is null)
                throw new ArgumentNullException("Doctor to update is null.");

            if (!(await _repository.ContainsAsync<Doctor>(x => x.Id == request.Id)))
                throw new KeyNotFoundException("Doctor with passed id does not exist.");

            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException("Doctors name cannot be blank.");

            if (string.IsNullOrEmpty(request.Surname))
                throw new ArgumentException("Doctors surname cannot be blank.");

            if (string.IsNullOrEmpty(request.Patronymic))
                throw new ArgumentException("Doctors patronymic cannot be blank.");

            if (request.DateOfBirth.CompareTo(DateOnly.FromDateTime(DateTime.Now.AddYears(-18))) > 0)
                throw new ArgumentException("Doctor must be at least 18 years old.");

            // Updating the doctor
            var doctor = request.ToDoctor();
            await _repository.UpdateAsync(doctor);

            return doctor.ToDoctorResponse();
        }
    }
}
