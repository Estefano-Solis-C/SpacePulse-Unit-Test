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
    public class CreateSpaceCommandHandlerTests
    {
        private readonly Mock<ISpaceRepository> _mockSpaceRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly SpaceAppService _spaceService;

        public CreateSpaceCommandHandlerTests()
        {
            _mockSpaceRepo = new Mock<ISpaceRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _spaceService = new SpaceAppService(_mockSpaceRepo.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task CreateSpaceAsync_ValidCommand_PersistsSpaceAndReturnsAggregate()
        {
            // Arrange
            var command = new CreateSpaceCommand(
                Title: "Miraflores Loft",
                Description: "Modern loft with IoT smart devices",
                Type: "Apartment",
                PricePerMonth: 1500m,
                Address: "Av. Larco 400",
                City: "Lima",
                Country: "Peru",
                Latitude: -12.122,
                Longitude: -77.028,
                Images: new string[] { "https://example.com/img.jpg" }
            );

            // Act
            var result = await _spaceService.CreateSpaceAsync(command);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Miraflores Loft", result.Title);
            Assert.Equal("Published", result.Status);
            _mockSpaceRepo.Verify(r => r.AddAsync(It.IsAny<Space>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}
