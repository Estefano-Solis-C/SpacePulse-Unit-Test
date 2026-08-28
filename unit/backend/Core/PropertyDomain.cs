using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Property.Domain.Model.Aggregates
{
    public class Space
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal PricePerMonth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string[] Images { get; set; } = Array.Empty<string>();
        public string Status { get; set; } = "Published";
        public Guid? RemodelerId { get; set; }

        public Space() { }

        public Space(string title, string description, string type, decimal pricePerMonth, string address, string city, string country, double latitude, double longitude, string[] images)
        {
            Title = title;
            Description = description;
            Type = type;
            PricePerMonth = pricePerMonth;
            Address = address;
            City = city;
            Country = country;
            Latitude = latitude;
            Longitude = longitude;
            Images = images;
            Status = "Published";
        }

        public void AcceptProject(Guid remodelerId)
        {
            Status = "Accepted";
            RemodelerId = remodelerId;
        }

        public void CompleteProject()
        {
            Status = "Completed";
        }
    }
}

namespace RentalPeAPI.Property.Domain.Repositories
{
    using RentalPeAPI.Property.Domain.Model.Aggregates;

    public interface ISpaceRepository
    {
        Task<Space?> FindByIdAsync(long id);
        Task AddAsync(Space space);
        void Update(Space space);
    }
}

namespace RentalPeAPI.Property.Application.Internal.CommandServices
{
    public record CreateSpaceCommand(
        string Title,
        string Description,
        string Type,
        decimal PricePerMonth,
        string Address,
        string City,
        string Country,
        double Latitude,
        double Longitude,
        string[] Images
    );

    public record AcceptSpaceCommand(long SpaceId, Guid RemodelerId);
    public record CompleteSpaceCommand(long SpaceId, Guid HomeownerId);
}

namespace RentalPeAPI.Property.Application.Services
{
    using RentalPeAPI.Property.Domain.Model.Aggregates;
    using RentalPeAPI.Property.Domain.Repositories;
    using RentalPeAPI.Property.Application.Internal.CommandServices;

    public class SpaceAppService
    {
        private readonly ISpaceRepository _spaceRepo;
        private readonly IUnitOfWork _unitOfWork;

        public SpaceAppService(ISpaceRepository spaceRepo, IUnitOfWork unitOfWork)
        {
            _spaceRepo = spaceRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Space> CreateSpaceAsync(CreateSpaceCommand command)
        {
            var space = new Space(
                command.Title,
                command.Description,
                command.Type,
                command.PricePerMonth,
                command.Address,
                command.City,
                command.Country,
                command.Latitude,
                command.Longitude,
                command.Images
            );
            await _spaceRepo.AddAsync(space);
            await _unitOfWork.CompleteAsync();
            return space;
        }

        public async Task<Space> AcceptProjectAsync(AcceptSpaceCommand command)
        {
            var space = await _spaceRepo.FindByIdAsync(command.SpaceId);
            if (space == null)
                throw new KeyNotFoundException($"Space {command.SpaceId} not found");

            space.AcceptProject(command.RemodelerId);
            _spaceRepo.Update(space);
            await _unitOfWork.CompleteAsync();
            return space;
        }

        public async Task<Space> CompleteProjectAsync(CompleteSpaceCommand command)
        {
            var space = await _spaceRepo.FindByIdAsync(command.SpaceId);
            if (space == null)
                throw new KeyNotFoundException($"Space {command.SpaceId} not found");

            space.CompleteProject();
            _spaceRepo.Update(space);
            await _unitOfWork.CompleteAsync();
            return space;
        }
    }
}
