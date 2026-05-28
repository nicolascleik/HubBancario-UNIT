using System;
using System.Threading.Tasks;
using HubBancario.Domain.Aggregates.Account;
using HubBancario.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HubBancario.Infrastructure.Persistence.Repositories
{
    public class ClientSecretRepository : IClientSecretRepository
    {
        private readonly BankingDbContext _context;

        public ClientSecretRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<ClientSecret> GetByIdAsync(Guid id)
        {
            return await _context.ClientSecrets.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(ClientSecret clientSecret)
        {
            await _context.ClientSecrets.AddAsync(clientSecret);
        }

        public Task UpdateAsync(ClientSecret clientSecret)
        {
            _context.ClientSecrets.Update(clientSecret);
            return Task.CompletedTask;
        }
    }
}