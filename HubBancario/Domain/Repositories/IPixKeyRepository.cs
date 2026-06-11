using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HubBancario.Domain.Aggregates.Account;

namespace HubBancario.Domain.Repositories
{
    public interface IPixKeyRepository
    {
        Task<PixKey> GetByIdAsync(Guid id);
        Task<IEnumerable<PixKey>> GetByAccountIdAsync(Guid accountId);
        Task AddAsync(PixKey pixKey);
        Task UpdateAsync(PixKey pixKey);
        Task DeleteAsync(PixKey pixKey);
        Task<PixKey> GetByKeyValueAsync(string keyValue);
    }
}