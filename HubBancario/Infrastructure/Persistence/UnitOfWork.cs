using System.Threading.Tasks;
using HubBancario.Domain.Repositories;

namespace HubBancario.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BankingDbContext _context;

        public UnitOfWork(BankingDbContext context)
        {
            _context = context;
        }

        public async Task CommitAsync()
        {
            // Consolida de forma atômica e assíncrona todas as modificações no banco
            await _context.SaveChangesAsync();
        }
    }
}