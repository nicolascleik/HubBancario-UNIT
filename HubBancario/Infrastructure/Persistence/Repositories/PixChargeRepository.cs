using System.Threading.Tasks;
using HubBancario.Domain.Aggregates.PixCharge;
using HubBancario.Domain.Repositories;
using HubBancario.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HubBancario.Infrastructure.Persistence.Repositories
{
    public class PixChargeRepository : IPixChargeRepository
    {
        private readonly BankingDbContext _context;

        public PixChargeRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<PixCharge> GetByTxIdAsync(TxId txId)
        {
            // O EF Core converte o record TxId automaticamente para a string da coluna tx_id no banco
            return await _context.PixCharges.FirstOrDefaultAsync(x => x.TxId == txId);
        }

        public async Task AddAsync(PixCharge pixCharge)
        {
            await _context.PixCharges.AddAsync(pixCharge);
        }

        public Task UpdateAsync(PixCharge pixCharge)
        {
            _context.PixCharges.Update(pixCharge);
            return Task.CompletedTask;
        }
    }
}