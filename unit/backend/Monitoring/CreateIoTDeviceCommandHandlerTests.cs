using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Tests.Unit.Monitoring
{
    public class CreateIoTDeviceCommandHandlerTests
    {
        private readonly Mock<IIoTDeviceRepository> _mockDeviceRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CreateIoTDeviceCommandHandler _handler;

        public CreateIoTDeviceCommandHandlerTests()
        {
            _mockDeviceRepo = new Mock<IIoTDeviceRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new CreateIoTDeviceCommandHandler(_mockDeviceRepo.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_CreatesDeviceAndCommitsUnitOfWork()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateIoTDeviceCommand(
                SpaceId: 10,
                CreatedByUserId: userId,
                Type: "Thermostat",
                Name: "Main Office Thermostat",
                SerialNumber: "SN-THERM-001",
                CustomMetricName: "Temperature",
                CustomUnit: "°C",
                CustomMinThreshold: 18.0m,
                CustomMaxThreshold: 26.0m
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Main Office Thermostat", result.Name);
            Assert.Equal("SN-THERM-001", result.SerialNumber);
            _mockDeviceRepo.Verify(r => r.AddAsync(It.IsAny<IoTDevice>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}
