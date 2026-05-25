using BankingHub.Domain.Aggregates.Invoice;
using BankingHub.Domain.Aggregates.PixCharge;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BankingHub.Infrastructure.Persistence;

/// <summary>
/// DbContext principal da aplicação.
/// Responsável por mapear os Aggregates do domínio para o banco de dados.
/// Implementa o padrão Unit of Work via SaveChangesAsync.
/// </summary>
public sealed class BankingDbContext : DbContext, IUnitOfWork
{
    private readonly IMediator _mediator;

    public BankingDbContext(
        DbContextOptions<BankingDbContext> options,
        IMediator mediator)
        : base(options)
    {
        _mediator = mediator;
    }

    // ── DbSets ──────────────────────────────────────────────────────────
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<PixCharge> PixCharges => Set<PixCharge>();

    // ── Configuração do Modelo ───────────────────────────────────────────
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica todas as IEntityTypeConfiguration<T> do assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankingDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    // ── Unit of Work ─────────────────────────────────────────────────────
    /// <summary>
    /// Persiste as mudanças e despacha os Domain Events após o commit.
    /// A ordem garante que os eventos só são disparados quando a transação
    /// já foi confirmada com sucesso no banco.
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Persiste no banco
        var result = await base.SaveChangesAsync(cancellationToken);

        // 2. Despacha Domain Events (após commit - eventos não são revertidos)
        await DispatchDomainEventsAsync(cancellationToken);

        return result;
    }

    // ── Dispatch de Domain Events ────────────────────────────────────────
    private async Task DispatchDomainEventsAsync(CancellationToken ct)
    {
        // Coleta todos os aggregates com eventos pendentes
        var aggregatesWithEvents = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        // Coleta todos os eventos antes de limpá-los
        var domainEvents = aggregatesWithEvents
            .SelectMany(a => a.DomainEvents)
            .ToList();

        // Limpa os eventos dos aggregates
        aggregatesWithEvents.ForEach(a => a.ClearDomainEvents());

        // Despacha cada evento via MediatR
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, ct);
        }
    }
}
