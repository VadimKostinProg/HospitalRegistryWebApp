using AutoFixture;
using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;
using HospitalReqistry.Domain.Entities;
using HospitalReqistry.Domain.RepositoryContracts;
using Moq;

namespace HospitalRegistry.Tests.PatientsServiceTests;

public abstract class PatientsServiceTestsBase
{
    protected readonly IPatientsService service;
    protected readonly Mock<IAsyncRepository> repositoryMock;
    protected readonly Fixture fixture;

    public PatientsServiceTestsBase()
    {
        fixture = new Fixture();

        repositoryMock = new Mock<IAsyncRepository>();

        service = new PatientsService(repositoryMock.Object);
    }

    public IEnumerable<Patient> GetTestPatients(int count = 10)
    {
        for (int i = 0; i < count; i++)
        {
            yield return GetTestPatient();
        }
    }

    public Patient GetTestPatient()
    {
        return new Patient()
        {
            Id = Guid.NewGuid(),
            Name = fixture.Create<string>(),
            Surname = fixture.Create<string>(),
            Patronymic = fixture.Create<string>(),
            DateOfBirth = new DateOnly(2000, 1, 1).ToString(),
            Email = fixture.Create<string>(),
            PhoneNumber = fixture.Create<string>()
        };
    }
}