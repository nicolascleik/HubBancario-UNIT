using System;
using System.Threading.Tasks;
using HubBancario.Domain.Aggregates.Invoice;
using HubBancario.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HubBancario.Infrastructure.Persistence.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly BankingDbContext _context;

        public InvoiceRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice> GetByIdAsync(Guid id)
        {
            return await _context.Invoices.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
        }

        public Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            return Task.CompletedTask;
        }
    }
}