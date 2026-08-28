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
    public class IoTTelemetryThresholdTests
    {
        private readonly Mock<IIoTDeviceRepository> _mockDeviceRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public IoTTelemetryThresholdTests()
        {
            _mockDeviceRepo = new Mock<IIoTDeviceRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
        }

        [Fact]
        public void IoTDevice_DefaultInitialization_SetsActiveStateToTrue()
        {
            // Arrange & Act
            var device = new IoTDevice(
                spaceId: 5,
                createdByUserId: Guid.NewGuid(),
                type: "Thermostat",
                name: "Smart Thermostat",
                serialNumber: "SN-TH-002",
                metricName: "Temperature",
                unit: "°C",
                minThreshold: 18.0m,
                maxThreshold: 26.0m
            );

            // Assert
            Assert.True(device.IsOn);
            Assert.Equal(18.0m, device.MinThreshold);
            Assert.Equal(26.0m, device.MaxThreshold);
        }

        [Fact]
        public void IoTDevice_TogglePowerMultipleTimes_CorrectlyAlternatesState()
        {
            // Arrange
            var device = new IoTDevice(1, Guid.NewGuid(), "Lighting", "Bulb", "SN-B1", "Lux", "lx", 50m, 500m);

            // Act & Assert
            Assert.True(device.IsOn);
            device.TogglePower();
            Assert.False(device.IsOn);
            device.TogglePower();
            Assert.True(device.IsOn);
        }

        [Fact]
        public async Task CreateWorkItem_CompletePayload_PersistsCorrectFields()
        {
            // Arrange
            var mockRepo = new Mock<IWorkItemRepository>();
            var handler = new CreateWorkItemCommandHandler(mockRepo.Object, _mockUnitOfWork.Object);
            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var command = new CreateWorkItemCommand(
                SpaceId: 25,
                CreatedByUserId: userId,
                Title: "Install Smart Sensor Pack",
                Description: "Mount 4 multi-sensors across living room",
                PhotoUrl: "https://cdn.spacepulse.com/img1.jpg",
                PlannedStartDate: now,
                PlannedEndDate: now.AddDays(3),
                Price: 350.00m
            );

            // Act
            var resultId = await handler.Handle(command, CancellationToken.None);

            // Assert
            mockRepo.Verify(r => r.AddAsync(It.Is<WorkItem>(w =>
                w.SpaceId == 25 &&
                w.CreatedByUserId == userId &&
                w.Title == "Install Smart Sensor Pack" &&
                w.Price == 350.00m
            )), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}
