using System;
using System.Threading.Tasks;
using HubBancario.Domain.Aggregates.Account;

namespace HubBancario.Domain.Repositories
{
    public interface IAccountRepository
    {
        Task<Account> GetByIdAsync(Guid id);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
    }
}