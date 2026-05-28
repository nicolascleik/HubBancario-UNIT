using System;
using System.Threading.Tasks;
using HubBancario.Domain.Aggregates.Account;
using HubBancario.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HubBancario.Infrastructure.Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingDbContext _context;

        public AccountRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<Account> GetByIdAsync(Guid id)
        {
            return await _context.Accounts.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
        }

        public Task UpdateAsync(Account account)
        {
            _context.Accounts.Update(account);
            return Task.CompletedTask;
        }
    }
}