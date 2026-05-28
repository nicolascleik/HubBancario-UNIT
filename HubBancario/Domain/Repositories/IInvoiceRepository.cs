using System;
using System.Threading.Tasks;
using HubBancario.Domain.Aggregates.Invoice;

namespace HubBancario.Domain.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Invoice> GetByIdAsync(Guid id);
        Task AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
    }
}

