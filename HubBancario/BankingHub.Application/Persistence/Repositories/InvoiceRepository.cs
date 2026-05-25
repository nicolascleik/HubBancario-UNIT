using BankingHub.Domain.Aggregates.Invoice;
using BankingHub.Domain.Repositories;
using BankingHub.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BankingHub.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação concreta do repositório de Invoice usando EF Core.
/// Esta classe vive na camada de Infrastructure e implementa a interface
/// definida no Domain, respeitando a regra de dependência da Clean Architecture.
/// </summary>
public sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly BankingDbContext _context;

    public InvoiceRepository(BankingDbContext context)
    {
        _context = context;
    }

    // ── Leitura ──────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public async Task<Invoice?> GetByIdAsync(
        InvoiceId id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Invoice?> GetByTxIdAsync(
        TxId txId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .FirstOrDefaultAsync(i => i.TxId == txId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Invoice?> GetByExternalReferenceAsync(
        string externalReference,
        CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .FirstOrDefaultAsync(
                i => i.ExternalReference == externalReference,
                cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Invoice>> GetOpenByBankAsync(
        string bankId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .Where(i => i.BankId == bankId && i.Status == InvoiceStatus.Open)
            .OrderBy(i => i.DueDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Invoice>> GetOverdueAsync(
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await _context.Invoices
            .Where(i => i.Status == InvoiceStatus.Open && i.DueDate < today)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Invoice>> GetPagedAsync(
        int page,
        int pageSize,
        InvoiceStatus? statusFilter = null,
        string? bankIdFilter = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Invoices.AsQueryable();

        if (statusFilter.HasValue)
            query = query.Where(i => i.Status == statusFilter.Value);

        if (!string.IsNullOrWhiteSpace(bankIdFilter))
            query = query.Where(i => i.BankId == bankIdFilter);

        return await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(
        InvoiceId id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .AnyAsync(i => i.Id == id, cancellationToken);
    }

    // ── Escrita ──────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public async Task AddAsync(
        Invoice invoice,
        CancellationToken cancellationToken = default)
    {
        await _context.Invoices.AddAsync(invoice, cancellationToken);
    }

    /// <inheritdoc/>
    public void Update(Invoice invoice)
    {
        // EF Core rastreia automaticamente as mudanças das entidades já carregadas.
        // O Update explícito é necessário apenas para entidades desanexadas (detached).
        _context.Invoices.Update(invoice);
    }
}
