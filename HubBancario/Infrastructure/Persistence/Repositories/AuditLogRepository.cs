using System.Threading.Tasks;
using HubBancario.Domain.Aggregates.Audit;
using HubBancario.Domain.Repositories;

namespace HubBancario.Infrastructure.Persistence.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly BankingDbContext _context;

        public AuditLogRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
        }
    }
}