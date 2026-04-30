using System;

namespace HubBancario.Domain.Aggregates.Audit
{
    public class AuditLog
    {
        public Guid Id { get; private set; }
        public Guid EntityId { get; private set; }
        public AuditAction Action { get; private set; }
        public string Changes { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // O AuditLog deve ser imutável após a criação. 
        // Por isso, deixamos apenas o esqueleto do construtor sem métodos de atualização.
        public AuditLog()
        {
            throw new NotImplementedException();
        }
    }
}

