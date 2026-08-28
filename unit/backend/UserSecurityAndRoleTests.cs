using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using RentalPeAPI.User.Application.Internal.CommandServices;
using RentalPeAPI.User.Application.Internal.QueryServices;
using RentalPeAPI.User.Domain.Model.Aggregates;
using RentalPeAPI.User.Domain.Repositories;
using RentalPeAPI.User.Domain.Services;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Tests.Unit.Security
{
    public class UserSecurityAndRoleTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IPasswordHashingService> _mockHasher;
        private readonly Mock<ITokenGenerationService> _mockTokenGen;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public UserSecurityAndRoleTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockHasher = new Mock<IPasswordHashingService>();
            _mockTokenGen = new Mock<ITokenGenerationService>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
        }

        [Fact]
        public async Task Register_DuplicateEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            var existingUser = new User("Existing User", "existing@spacepulse.com", "hash", "+51999999999", "Homeowner", "");
            _mockUserRepo.Setup(r => r.FindByEmailAsync("existing@spacepulse.com")).ReturnsAsync(existingUser);

            var handler = new RentalPeAPI.User.Application.Internal.EventHandlers.RegisterUserCommandHandler(_mockUserRepo.Object, _mockHasher.Object, _mockUnitOfWork.Object);
            var command = new RegisterUserCommand("New User", "existing@spacepulse.com", "Pass123!", "+51999111222", "Remodeler", "");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task Login_NonExistentUser_ThrowsException()
        {
            // Arrange
            _mockUserRepo.Setup(r => r.FindByEmailAsync("unknown@spacepulse.com")).ReturnsAsync((User?)null);
            var handler = new LoginQueryHandler(_mockUserRepo.Object, _mockHasher.Object, _mockTokenGen.Object);
            var query = new LoginQuery("unknown@spacepulse.com", "Password123!");

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(query, CancellationToken.None));
        }

        [Theory]
        [InlineData("Homeowner")]
        [InlineData("Remodeler")]
        public async Task Register_SupportedRoles_CreatesUserWithCorrectRole(string role)
        {
            // Arrange
            _mockUserRepo.Setup(r => r.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _mockHasher.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("secure_hash");

            var handler = new RentalPeAPI.User.Application.Internal.EventHandlers.RegisterUserCommandHandler(_mockUserRepo.Object, _mockHasher.Object, _mockUnitOfWork.Object);
            var command = new RegisterUserCommand("Test User", $"{role.ToLower()}@spacepulse.com", "Pass123!", "+51999000111", role, "");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(role, result.Role);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}
