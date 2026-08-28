using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Domain.Model.Aggregates
{
    public class IoTDevice
    {
        public long Id { get; set; }
        public long SpaceId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string MetricName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal MinThreshold { get; set; }
        public decimal MaxThreshold { get; set; }
        public bool IsOn { get; set; } = true;

        public IoTDevice() { }

        public IoTDevice(long spaceId, Guid createdByUserId, string type, string name, string serialNumber, string metricName, string unit, decimal minThreshold, decimal maxThreshold)
        {
            SpaceId = spaceId;
            CreatedByUserId = createdByUserId;
            Type = type;
            Name = name;
            SerialNumber = serialNumber;
            MetricName = metricName;
            Unit = unit;
            MinThreshold = minThreshold;
            MaxThreshold = maxThreshold;
            IsOn = true;
        }

        public void TogglePower()
        {
            IsOn = !IsOn;
        }
    }

    public class WorkItem
    {
        public long Id { get; set; }
        public long SpaceId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = "Pending";
    }
}

namespace RentalPeAPI.Monitoring.Domain.Repositories
{
    using RentalPeAPI.Monitoring.Domain.Model.Aggregates;

    public interface IIoTDeviceRepository
    {
        Task<IoTDevice?> FindByIdAsync(long id);
        Task AddAsync(IoTDevice device);
        void Update(IoTDevice device);
    }

    public interface IWorkItemRepository
    {
        Task<WorkItem?> FindByIdAsync(long id);
        Task AddAsync(WorkItem item);
        void Update(WorkItem item);
    }
}

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices
{
    using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
    using RentalPeAPI.Monitoring.Domain.Repositories;

    public record CreateIoTDeviceCommand(
        long SpaceId,
        Guid CreatedByUserId,
        string Type,
        string Name,
        string SerialNumber,
        string? CustomMetricName,
        string? CustomUnit,
        decimal? CustomMinThreshold,
        decimal? CustomMaxThreshold
    );

    public record ToggleIoTDevicePowerCommand(long DeviceId, Guid UserId);

    public record CreateWorkItemCommand(
        long SpaceId,
        Guid CreatedByUserId,
        string Title,
        string Description,
        string? PhotoUrl,
        DateTime? PlannedStartDate,
        DateTime? PlannedEndDate,
        decimal Price
    );

    public class CreateIoTDeviceCommandHandler
    {
        private readonly IIoTDeviceRepository _deviceRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CreateIoTDeviceCommandHandler(IIoTDeviceRepository deviceRepo, IUnitOfWork unitOfWork)
        {
            _deviceRepo = deviceRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IoTDevice> Handle(CreateIoTDeviceCommand command, CancellationToken cancellationToken)
        {
            var device = new IoTDevice(
                command.SpaceId,
                command.CreatedByUserId,
                command.Type,
                command.Name,
                command.SerialNumber,
                command.CustomMetricName ?? "Temperature",
                command.CustomUnit ?? "°C",
                command.CustomMinThreshold ?? 18m,
                command.CustomMaxThreshold ?? 26m
            );
            await _deviceRepo.AddAsync(device);
            await _unitOfWork.CompleteAsync();
            return device;
        }
    }

    public class ToggleIoTDevicePowerCommandHandler
    {
        private readonly IIoTDeviceRepository _deviceRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ToggleIoTDevicePowerCommandHandler(IIoTDeviceRepository deviceRepo, IUnitOfWork unitOfWork)
        {
            _deviceRepo = deviceRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IoTDevice> Handle(ToggleIoTDevicePowerCommand command, CancellationToken cancellationToken)
        {
            var device = await _deviceRepo.FindByIdAsync(command.DeviceId);
            if (device == null)
                throw new KeyNotFoundException($"Device {command.DeviceId} not found");

            if (device.CreatedByUserId != command.UserId)
                throw new UnauthorizedAccessException("Unauthorized access to IoT device");

            device.TogglePower();
            _deviceRepo.Update(device);
            await _unitOfWork.CompleteAsync();
            return device;
        }
    }

    public class CreateWorkItemCommandHandler
    {
        private readonly IWorkItemRepository _workItemRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CreateWorkItemCommandHandler(IWorkItemRepository workItemRepo, IUnitOfWork unitOfWork)
        {
            _workItemRepo = workItemRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateWorkItemCommand command, CancellationToken cancellationToken)
        {
            var workItem = new WorkItem
            {
                SpaceId = command.SpaceId,
                CreatedByUserId = command.CreatedByUserId,
                Title = command.Title,
                Description = command.Description,
                PhotoUrl = command.PhotoUrl,
                PlannedStartDate = command.PlannedStartDate,
                PlannedEndDate = command.PlannedEndDate,
                Price = command.Price
            };
            await _workItemRepo.AddAsync(workItem);
            await _unitOfWork.CompleteAsync();
            return workItem.Id;
        }
    }
}
