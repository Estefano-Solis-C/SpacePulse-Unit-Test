using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using RentalPeAPI.User.Application.Internal.QueryServices;
using RentalPeAPI.User.Domain.Model.Aggregates;
using RentalPeAPI.User.Domain.Repositories;
using RentalPeAPI.User.Domain.Services;

namespace RentalPeAPI.Tests.Unit.User
{
    public class LoginQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IPasswordHashingService> _mockHasher;
        private readonly Mock<ITokenGenerationService> _mockTokenGen;
        private readonly LoginQueryHandler _handler;

        public LoginQueryHandlerTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockHasher = new Mock<IPasswordHashingService>();
            _mockTokenGen = new Mock<ITokenGenerationService>();
            _handler = new LoginQueryHandler(_mockUserRepo.Object, _mockHasher.Object, _mockTokenGen.Object);
        }

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsAuthenticationDtoWithToken()
        {
            // Arrange
            var user = new RentalPeAPI.User.Domain.Model.Aggregates.User("Carlos", "carlos@spacepulse.com", "hashed_pwd", "+51999888777", "Homeowner", "");
            _mockUserRepo.Setup(r => r.FindByEmailAsync("carlos@spacepulse.com")).ReturnsAsync(user);
            _mockHasher.Setup(h => h.VerifyPassword("Password123!", "hashed_pwd")).Returns(true);
            _mockTokenGen.Setup(t => t.GenerateToken(user)).Returns("jwt_sample_token_xyz");

            var query = new LoginQuery("carlos@spacepulse.com", "Password123!");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("jwt_sample_token_xyz", result.Token);
            Assert.Equal("Carlos", result.FullName);
        }

        [Fact]
        public async Task Handle_InvalidPassword_ThrowsException()
        {
            // Arrange
            var user = new RentalPeAPI.User.Domain.Model.Aggregates.User("Carlos", "carlos@spacepulse.com", "hashed_pwd", "+51999888777", "Homeowner", "");
            _mockUserRepo.Setup(r => r.FindByEmailAsync("carlos@spacepulse.com")).ReturnsAsync(user);
            _mockHasher.Setup(h => h.VerifyPassword("WrongPassword", "hashed_pwd")).Returns(false);

            var query = new LoginQuery("carlos@spacepulse.com", "WrongPassword");

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
