using HospitalRegistry.Infrastructure.DatabaseContexts;
using HospitalRegistry.Infrastructure.Repositories;
using HospitalReqistry.Application.RepositoryContracts;
using HospitalReqistry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistry.Tests.RepositoryTests;

public abstract class RepositoryTestsBase : HospitalRegistryTestsBase
{
	protected readonly ApplicationContext context;
    protected readonly IAsyncRepository repository;
	protected readonly List<Doctor> doctorsList;

	public RepositoryTestsBase()
	{
		var options = new DbContextOptionsBuilder<ApplicationContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		context = new ApplicationContext(options);

		doctorsList = GetTestDoctors().ToList();

        doctorsList[0].Name = doctorsList[1].Name = doctorsList[2].Name = "test name";

        context.AddRange(doctorsList);
		context.SaveChanges();

		this.repository = new RepositoryBase(context);
	}
}