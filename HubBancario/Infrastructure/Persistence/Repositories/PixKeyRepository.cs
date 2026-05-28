using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HubBancario.Domain.Aggregates.Account;
using HubBancario.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HubBancario.Infrastructure.Persistence.Repositories
{
    public class PixKeyRepository : IPixKeyRepository
    {
        private readonly BankingDbContext _context;

        public PixKeyRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<PixKey> GetByIdAsync(Guid id)
        {
            return await _context.PixKeys.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<PixKey>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.PixKeys
                .Where(x => x.AccountId == accountId)
                .ToListAsync();
        }

        public async Task AddAsync(PixKey pixKey)
        {
            await _context.PixKeys.AddAsync(pixKey);
        }

        public Task UpdateAsync(PixKey pixKey)
        {
            _context.PixKeys.Update(pixKey);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(PixKey pixKey)
        {
            // Implementação de exclusão física (Hard Delete). 
            // Lembrando que a exclusão lógica (Soft Delete) já é tratada pelo UpdateAsync.
            _context.PixKeys.Remove(pixKey);
            return Task.CompletedTask;
        }
    }
}