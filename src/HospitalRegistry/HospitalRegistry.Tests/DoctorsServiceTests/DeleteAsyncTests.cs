using HospitalReqistry.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalRegistry.Tests.DoctorsServiceTests
{
    public class DeleteAsyncTests : DoctorsServiceTestsBase
    {
        public DeleteAsyncTests() : base() { }

        [Fact]
        public async Task DeleteAsync_InvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var idToPass = Guid.NewGuid();
            repositoryMock.Setup(x => x.DeleteAsync<Doctor>(idToPass)).ReturnsAsync(false);

            // Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                // Act
                await service.DeleteAsync(idToPass);
            });
        }
    }
}
