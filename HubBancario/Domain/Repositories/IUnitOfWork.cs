using System.Threading.Tasks;

namespace HubBancario.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}

