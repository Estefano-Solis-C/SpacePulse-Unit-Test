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
    public class WorkItemCommandHandlerTests
    {
        private readonly Mock<IWorkItemRepository> _mockWorkItemRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CreateWorkItemCommandHandler _createHandler;

        public WorkItemCommandHandlerTests()
        {
            _mockWorkItemRepo = new Mock<IWorkItemRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _createHandler = new CreateWorkItemCommandHandler(_mockWorkItemRepo.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_CreateWorkItemRequest_PersistsAndReturnsId()
        {
            // Arrange
            var command = new CreateWorkItemCommand(
                SpaceId: 5,
                CreatedByUserId: Guid.NewGuid(),
                Title: "Air conditioner maintenance",
                Description: "Filter cleaning requested",
                PhotoUrl: null,
                PlannedStartDate: null,
                PlannedEndDate: null,
                Price: 0m
            );

            // Act
            var result = await _createHandler.Handle(command, CancellationToken.None);

            // Assert
            _mockWorkItemRepo.Verify(r => r.AddAsync(It.IsAny<WorkItem>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}
