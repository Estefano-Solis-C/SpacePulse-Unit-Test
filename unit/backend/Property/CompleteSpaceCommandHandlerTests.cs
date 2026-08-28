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
    public class CompleteSpaceCommandHandlerTests
    {
        private readonly Mock<ISpaceRepository> _mockSpaceRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly SpaceAppService _spaceService;

        public CompleteSpaceCommandHandlerTests()
        {
            _mockSpaceRepo = new Mock<ISpaceRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _spaceService = new SpaceAppService(_mockSpaceRepo.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task CompleteProjectAsync_AcceptedSpace_TransitionsToCompleted()
        {
            // Arrange
            long spaceId = 20;
            var homeownerId = Guid.NewGuid();
            var remodelerId = Guid.NewGuid();
            var space = new Space("Loft B", "Desc", "Apartment", 1500m, "Addr", "Lima", "Peru", 0, 0, new string[0]);
            space.AcceptProject(remodelerId);

            _mockSpaceRepo.Setup(r => r.FindByIdAsync(spaceId)).ReturnsAsync(space);

            var command = new CompleteSpaceCommand(spaceId, homeownerId);

            // Act
            var result = await _spaceService.CompleteProjectAsync(command);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Completed", result.Status);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}
