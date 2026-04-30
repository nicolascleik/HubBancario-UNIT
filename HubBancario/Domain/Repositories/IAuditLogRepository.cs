using System.Threading.Tasks;
using HubBancario.Domain.Aggregates.Audit;

namespace HubBancario.Domain.Repositories
{
    public interface IAuditLogRepository
    {
        // Logs de auditoria são imutáveis e "append-only" (apenas inserção)
        Task AddAsync(AuditLog auditLog);
    }
}

