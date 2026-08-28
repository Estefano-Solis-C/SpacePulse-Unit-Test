using System;
using System.Threading;
using System.Threading.Tasks;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.User.Domain.Model.Aggregates
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = "Homeowner";
        public string ProfilePhotoUrl { get; set; } = string.Empty;

        public User() { }

        public User(string fullName, string email, string passwordHash, string phoneNumber, string role, string profilePhotoUrl)
        {
            FullName = fullName;
            Email = email;
            PasswordHash = passwordHash;
            PhoneNumber = phoneNumber;
            Role = role;
            ProfilePhotoUrl = profilePhotoUrl;
        }
    }
}

namespace RentalPeAPI.User.Domain.Repositories
{
    using RentalPeAPI.User.Domain.Model.Aggregates;

    public interface IUserRepository
    {
        Task<User?> FindByEmailAsync(string email);
        Task<User?> FindByIdAsync(Guid id);
        Task AddAsync(User user);
    }
}

namespace RentalPeAPI.User.Domain.Services
{
    using RentalPeAPI.User.Domain.Model.Aggregates;

    public interface IPasswordHashingService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }

    public interface ITokenGenerationService
    {
        string GenerateToken(User user);
    }
}

namespace RentalPeAPI.User.Application.Internal.CommandServices
{
    public record RegisterUserCommand(
        string FullName,
        string Email,
        string Password,
        string PhoneNumber,
        string Role,
        string ProfilePhotoUrl
    );

    public record UserDto(Guid Id, string FullName, string Email, string Role);
}

namespace RentalPeAPI.User.Application.Internal.EventHandlers
{
    using RentalPeAPI.User.Domain.Model.Aggregates;
    using RentalPeAPI.User.Domain.Repositories;
    using RentalPeAPI.User.Domain.Services;
    using RentalPeAPI.User.Application.Internal.CommandServices;

    public class RegisterUserCommandHandler
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHashingService _hasher;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserCommandHandler(IUserRepository userRepo, IPasswordHashingService hasher, IUnitOfWork unitOfWork)
        {
            _userRepo = userRepo;
            _hasher = hasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDto> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var existing = await _userRepo.FindByEmailAsync(command.Email);
            if (existing != null)
                throw new InvalidOperationException("User email already registered");

            string hash = _hasher.HashPassword(command.Password);
            var user = new User(command.FullName, command.Email, hash, command.PhoneNumber, command.Role, command.ProfilePhotoUrl);
            await _userRepo.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return new UserDto(user.Id, user.FullName, user.Email, user.Role);
        }
    }
}

namespace RentalPeAPI.User.Application.Internal.QueryServices
{
    using RentalPeAPI.User.Domain.Model.Aggregates;
    using RentalPeAPI.User.Domain.Repositories;
    using RentalPeAPI.User.Domain.Services;

    public record LoginQuery(string Email, string Password);
    public record AuthenticationDto(string Token, string FullName, string Email, string Role);

    public class LoginQueryHandler
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHashingService _hasher;
        private readonly ITokenGenerationService _tokenGen;

        public LoginQueryHandler(IUserRepository userRepo, IPasswordHashingService hasher, ITokenGenerationService tokenGen)
        {
            _userRepo = userRepo;
            _hasher = hasher;
            _tokenGen = tokenGen;
        }

        public async Task<AuthenticationDto> Handle(LoginQuery query, CancellationToken cancellationToken)
        {
            var user = await _userRepo.FindByEmailAsync(query.Email);
            if (user == null)
                throw new Exception("Invalid email or password");

            bool valid = _hasher.VerifyPassword(query.Password, user.PasswordHash);
            if (!valid)
                throw new Exception("Invalid email or password");

            string token = _tokenGen.GenerateToken(user);
            return new AuthenticationDto(token, user.FullName, user.Email, user.Role);
        }
    }
}
