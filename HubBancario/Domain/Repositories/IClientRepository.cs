using System;
using System.Threading.Tasks;
using HubBancario.Domain.Aggregates.Client;

namespace HubBancario.Domain.Repositories
{
    public interface IClientRepository
    {
        Task<Client> GetByIdAsync(Guid id);
        Task AddAsync(Client client);
        Task UpdateAsync(Client client);
    }
}

