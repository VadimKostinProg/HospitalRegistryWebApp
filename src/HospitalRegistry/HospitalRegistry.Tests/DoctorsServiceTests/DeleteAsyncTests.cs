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
        [Fact]
        public async Task DeleteAsync_InvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var idToPass = Guid.NewGuid();
            repositoryMock.Setup(x => x.GetByIdAsync<Doctor>(idToPass, true))
                .ReturnsAsync(null as Doctor);

            // Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                // Act
                await service.DeleteAsync(idToPass);
            });
        }
    }
}
