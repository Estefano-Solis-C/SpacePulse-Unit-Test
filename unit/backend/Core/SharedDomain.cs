using System.Threading.Tasks;

namespace RentalPeAPI.Shared.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}
