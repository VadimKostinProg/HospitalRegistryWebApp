using AutoFixture;
using HospitalRegistry.Application.ServiceContracts;
using HospitalRegistry.Application.Services;
using HospitalReqistry.Domain.Entities;
using HospitalReqistry.Domain.RepositoryContracts;
using Moq;

namespace HospitalRegistry.Tests.DiagnosesServiceTests;

public abstract class DiagnosesServiceTestsBase
{
    protected IDiagnosesService service;
    protected Mock<IAsyncRepository> repositoryMock;
    protected Fixture fixture;

    public DiagnosesServiceTestsBase()
    {
        fixture = new Fixture();

        repositoryMock = new Mock<IAsyncRepository>();

        service = new DiagnosesService(repositoryMock.Object);
    }

    public IEnumerable<Diagnosis> GetDiagnoses(int count = 10)
    {
        for (int i = 0; i < count; i++)
            yield return GetDiagnosis();
    }

    public Diagnosis GetDiagnosis()
    {
        return new Diagnosis()
        {
            Id = Guid.NewGuid(),
            Name = fixture.Create<string>()
        };
    }
}