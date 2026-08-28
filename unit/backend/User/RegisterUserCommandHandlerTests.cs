using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using RentalPeAPI.User.Application.Internal.CommandServices;
using RentalPeAPI.User.Application.Internal.EventHandlers;
using RentalPeAPI.User.Domain.Model.Aggregates;
using RentalPeAPI.User.Domain.Repositories;
using RentalPeAPI.User.Domain.Services;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Tests.Unit.User
{
    public class RegisterUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IPasswordHashingService> _mockHasher;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly RegisterUserCommandHandler _handler;

        public RegisterUserCommandHandlerTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockHasher = new Mock<IPasswordHashingService>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new RegisterUserCommandHandler(_mockUserRepo.Object, _mockHasher.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_NewEmail_HashesPasswordAndRegistersUser()
        {
            // Arrange
            var command = new RegisterUserCommand("Carlos Sanchez", "carlos@spacepulse.com", "Password123!", "+51999888777", "Homeowner", "https://photo.jpg");

            _mockUserRepo.Setup(r => r.FindByEmailAsync("carlos@spacepulse.com")).ReturnsAsync((RentalPeAPI.User.Domain.Model.Aggregates.User?)null);
            _mockHasher.Setup(h => h.HashPassword("Password123!")).Returns("hashed_pass_string");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Carlos Sanchez", result.FullName);
            Assert.Equal("carlos@spacepulse.com", result.Email);
            Assert.Equal("Homeowner", result.Role);
            _mockUserRepo.Verify(r => r.AddAsync(It.IsAny<RentalPeAPI.User.Domain.Model.Aggregates.User>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}
