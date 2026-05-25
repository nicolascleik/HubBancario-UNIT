using BankingHub.Domain.Aggregates.PixCharge;
using BankingHub.Domain.Repositories;
using BankingHub.Domain.ValueObjects;
using BankingHub.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingHub.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação concreta do repositório de PixCharge usando EF Core.
/// Concentra todas as queries relacionadas às cobranças Pix,
/// incluindo as queries usadas pelos jobs de reconciliação e polling.
/// </summary>
public sealed class PixChargeRepository : IPixChargeRepository
{
    private readonly BankingDbContext _context;

    public PixChargeRepository(BankingDbContext context)
    {
        _context = context;
    }

    // ── Leitura ──────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public async Task<PixCharge?> GetByIdAsync(
        ChargeId id,
        CancellationToken cancellationToken = default)
    {
        return await _context.PixCharges
            .FirstOrDefaultAsync(pc => pc.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// TxId é a chave universal de conciliação do Pix (REGRA 3.1.3).
    /// Este método é chamado com frequência pelo ReconciliationJob e ProcessWebhookHandler.
    /// </remarks>
    public async Task<PixCharge?> GetByTxIdAsync(
        TxId txId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PixCharges
            .FirstOrDefaultAsync(pc => pc.TxId == txId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<PixCharge>> GetByInvoiceIdAsync(
        InvoiceId invoiceId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PixCharges
            .Where(pc => pc.InvoiceId == invoiceId)
            .OrderByDescending(pc => pc.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Usado pelo PollingJob para verificar cobranças ainda abertas.
    /// O índice ix_pix_charges_bank_status garante performance desta query.
    /// </remarks>
    public async Task<IReadOnlyList<PixCharge>> GetActivePendingReconciliationAsync(
        string bankId,
        TimeSpan olderThan,
        int batchSize = 100,
        CancellationToken cancellationToken = default)
    {
        var threshold = DateTime.UtcNow.Subtract(olderThan);

        return await _context.PixCharges
            .Where(pc =>
                pc.BankId == bankId &&
                pc.Status == PixChargeStatus.Active &&
                pc.CreatedAt <= threshold)
            .OrderBy(pc => pc.CreatedAt)   // mais antigas primeiro
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Usado pelo PollingJob para encontrar cobranças que estão
    /// prestes a expirar mas ainda não foram reconciliadas.
    /// O índice parcial ix_pix_charges_expires_at é otimizado para esta query.
    /// </remarks>
    public async Task<IReadOnlyList<PixCharge>> GetExpiringActiveAsync(
        TimeSpan expiresWithin,
        CancellationToken cancellationToken = default)
    {
        var deadline = DateTime.UtcNow.Add(expiresWithin);

        return await _context.PixCharges
            .Where(pc =>
                pc.Status == PixChargeStatus.Active &&
                pc.ExpiresAt.HasValue &&
                pc.ExpiresAt.Value <= deadline)
            .OrderBy(pc => pc.ExpiresAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsByTxIdAsync(
        TxId txId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PixCharges
            .AnyAsync(pc => pc.TxId == txId, cancellationToken);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Idempotência: verifica se já existe cobrança PAGA para o txId
    /// antes de reprocessar um webhook ou evento duplicado (REGRA 3.1.4).
    /// </remarks>
    public async Task<bool> IsAlreadyPaidAsync(
        TxId txId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PixCharges
            .AnyAsync(
                pc => pc.TxId == txId && pc.Status == PixChargeStatus.Paid,
                cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> CountByBankAndStatusAsync(
        string bankId,
        PixChargeStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _context.PixCharges
            .CountAsync(
                pc => pc.BankId == bankId && pc.Status == status,
                cancellationToken);
    }

    // ── Escrita ──────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public async Task AddAsync(
        PixCharge pixCharge,
        CancellationToken cancellationToken = default)
    {
        await _context.PixCharges.AddAsync(pixCharge, cancellationToken);
    }

    /// <inheritdoc/>
    public void Update(PixCharge pixCharge)
    {
        _context.PixCharges.Update(pixCharge);
    }
}
