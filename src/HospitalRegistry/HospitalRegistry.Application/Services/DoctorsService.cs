using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.Enums;
using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Specifications;
using HospitalReqistry.Application.RepositoryContracts;
using HospitalReqistry.Domain.Entities;

namespace HospitalRegistry.Application.Services
{
    public class DoctorsService : IDoctorsService
    {
        private readonly IAsyncRepository _repository;

        public DoctorsService(IAsyncRepository repository)
        {
            _repository = repository;
        }

        public async Task<ListModel<DoctorResponse>> GetDoctorsListAsync(DoctorSpecificationsDTO specificationsDTO)
        {
            if (specificationsDTO is null)
                throw new ArgumentNullException("Specifications are null.");

            return await this.GetDoctorsListBySpecificationAsync(this.GetSpecification(specificationsDTO));
        }

        private ISpecification<Doctor> GetSpecification(DoctorSpecificationsDTO specificationsDTO, bool isDeleted = false)
        {
            var builder = new SpecificationBuilder<Doctor>();

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

                if (specificationsDTO.Specialty is not null)
                    builder.With(x => x.Specialty == specificationsDTO.Specialty.ToString());

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
                        case "Specialty":
                            builder.OrderBy(x => x.Specialty, specificationsDTO.SortDirection ?? Enums.SortDirection.ASC);
                            break;
                    }
                }

                builder.WithPagination(specificationsDTO.PageSize, specificationsDTO.PageNumber);
            }

            return builder.Build();
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

            if (doctor.IsDeleted)
                throw new ArgumentException("Doctor is already deleted.");

            // Deleting doctors schedule
            var scheduleSlots = doctor.Schedules;

            if (scheduleSlots is not null)
            {
                await _repository.DeleteRangeAsync(scheduleSlots);
            }

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

        public async Task<DoctorResponse> GetByIdAsync(Guid id)
        {
            var doctor = await _repository.FirstOrDefaultAsync<Doctor>(x => x.Id == id && x.IsDeleted == false);

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

        public async Task<ListModel<DoctorResponse>> GetDeletedDoctorsListAsync(DoctorSpecificationsDTO specificationsDTO)
        {
            if (specificationsDTO is null)
                throw new ArgumentNullException("Specifications are null.");

            return await this.GetDoctorsListBySpecificationAsync(this.GetSpecification(specificationsDTO, isDeleted: true));
        }

        private async Task<ListModel<DoctorResponse>> GetDoctorsListBySpecificationAsync(ISpecification<Doctor> specification)
        {
            var doctors = (await _repository.GetAsync<Doctor>(specification, disableTracking: false))
                .Select(x => x.ToDoctorResponse())
                .ToList();

            var totalCount = specification.Predicate is null ?
                await _repository.CountAsync<Doctor>() :
                await _repository.CountAsync<Doctor>(specification.Predicate);

            var totalPages = (int)Math.Ceiling((double)totalCount / specification.Take);

            return new ListModel<DoctorResponse>
            {
                List = doctors,
                TotalPages = totalPages,
            };
        }
    }
}
