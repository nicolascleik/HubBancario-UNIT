using System;
using System.Threading.Tasks;
using HubBancario.Domain.Aggregates.Account;

namespace HubBancario.Domain.Repositories
{
    public interface IClientSecretRepository
    {
        Task<ClientSecret> GetByIdAsync(Guid id);
        Task AddAsync(ClientSecret clientSecret);
        Task UpdateAsync(ClientSecret clientSecret);
    }
}