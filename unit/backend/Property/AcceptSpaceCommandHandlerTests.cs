using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using RentalPeAPI.Property.Application.Services;
using RentalPeAPI.Property.Application.Internal.CommandServices;
using RentalPeAPI.Property.Domain.Model.Aggregates;
using RentalPeAPI.Property.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Tests.Unit.Property
{
    public class AcceptSpaceCommandHandlerTests
    {
        private readonly Mock<ISpaceRepository> _mockSpaceRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly SpaceAppService _spaceService;

        public AcceptSpaceCommandHandlerTests()
        {
            _mockSpaceRepo = new Mock<ISpaceRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _spaceService = new SpaceAppService(_mockSpaceRepo.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task AcceptProjectAsync_PublishedSpace_TransitionsToAcceptedAndAssignsRemodeler()
        {
            // Arrange
            long spaceId = 10;
            var remodelerId = Guid.NewGuid();
            var space = new Space("Loft A", "Desc", "Apartment", 1200m, "Addr", "Lima", "Peru", 0, 0, new string[0]);

            _mockSpaceRepo.Setup(r => r.FindByIdAsync(spaceId)).ReturnsAsync(space);

            var command = new AcceptSpaceCommand(spaceId, remodelerId);

            // Act
            var result = await _spaceService.AcceptProjectAsync(command);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Accepted", result.Status);
            Assert.Equal(remodelerId, result.RemodelerId);
            _mockSpaceRepo.Verify(r => r.Update(space), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}
