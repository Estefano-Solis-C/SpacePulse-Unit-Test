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
    public class ToggleIoTDevicePowerCommandHandlerTests
    {
        private readonly Mock<IIoTDeviceRepository> _mockDeviceRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly ToggleIoTDevicePowerCommandHandler _handler;

        public ToggleIoTDevicePowerCommandHandlerTests()
        {
            _mockDeviceRepo = new Mock<IIoTDeviceRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new ToggleIoTDevicePowerCommandHandler(_mockDeviceRepo.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ExistingDeviceAndMatchingCreator_TogglesPowerState()
        {
            // Arrange
            long deviceId = 100;
            var creatorUserId = Guid.NewGuid();
            var device = new IoTDevice(
                spaceId: 1,
                createdByUserId: creatorUserId,
                type: "AirConditioning",
                name: "HVAC Unit",
                serialNumber: "SN-100",
                metricName: "Temperature",
                unit: "°C",
                minThreshold: 18m,
                maxThreshold: 26m
            );
            bool initialPowerState = device.IsOn;

            _mockDeviceRepo.Setup(r => r.FindByIdAsync(deviceId)).ReturnsAsync(device);

            var command = new ToggleIoTDevicePowerCommand(deviceId, creatorUserId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(initialPowerState, result.IsOn);
            _mockDeviceRepo.Verify(r => r.Update(device), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DifferentUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            long deviceId = 100;
            var creatorUserId = Guid.NewGuid();
            var unauthorizedUserId = Guid.NewGuid();
            var device = new IoTDevice(
                spaceId: 1,
                createdByUserId: creatorUserId,
                type: "Lighting",
                name: "Living Room Lamp",
                serialNumber: "SN-200",
                metricName: "Brightness",
                unit: "Lux",
                minThreshold: 100m,
                maxThreshold: 800m
            );

            _mockDeviceRepo.Setup(r => r.FindByIdAsync(deviceId)).ReturnsAsync(device);

            var command = new ToggleIoTDevicePowerCommand(deviceId, unauthorizedUserId);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_DeviceNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            long nonExistentId = 999;
            _mockDeviceRepo.Setup(r => r.FindByIdAsync(nonExistentId)).ReturnsAsync((IoTDevice?)null);

            var command = new ToggleIoTDevicePowerCommand(nonExistentId, Guid.NewGuid());

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
