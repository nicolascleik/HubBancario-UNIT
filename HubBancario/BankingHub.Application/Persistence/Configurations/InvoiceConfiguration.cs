using BankingHub.Domain.Aggregates.Invoice;
using BankingHub.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingHub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração EF Core para o Aggregate Root Invoice.
/// Define o mapeamento entre o modelo de domínio e a tabela do banco de dados,
/// incluindo Value Objects (Money, TxId) como owned entities ou conversores.
/// </summary>
public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        // ── Tabela ───────────────────────────────────────────────────────
        builder.ToTable("invoices");

        // ── Chave Primária (Strongly Typed ID) ───────────────────────────
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,          // InvoiceId → Guid
                value => InvoiceId.From(value)) // Guid → InvoiceId
            .IsRequired();

        // ── Value Object: Money (Amount) ─────────────────────────────────
        // Money é um Value Object com dois campos: Value e Currency.
        // Mapeado como owned entity para manter o encapsulamento do domínio.
        builder.OwnsOne(i => i.Amount, money =>
        {
            money.Property(m => m.Value)
                .HasColumnName("amount")
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("BRL");
        });

        // ── Value Object: TxId (nullable) ────────────────────────────────
        builder.Property(i => i.TxId)
            .HasColumnName("tx_id")
            .HasConversion(
                txId => txId == null ? null : txId.Value,  // TxId? → string?
                value => value == null ? null : TxId.From(value)) // string? → TxId?
            .HasMaxLength(35)
            .IsRequired(false);

        // ── Propriedades Simples ─────────────────────────────────────────
        builder.Property(i => i.DueDate)
            .HasColumnName("due_date")
            .IsRequired();

        builder.Property(i => i.Status)
            .HasColumnName("status")
            .HasConversion<string>()   // Enum → string (legível no banco)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(i => i.BankId)
            .HasColumnName("bank_id")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(i => i.ExternalReference)
            .HasColumnName("external_reference")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(i => i.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(i => i.PaidAt)
            .HasColumnName("paid_at")
            .IsRequired(false);

        // ── Índices ──────────────────────────────────────────────────────
        // TxId: busca frequente para conciliação
        builder.HasIndex(i => i.TxId)
            .HasDatabaseName("ix_invoices_tx_id")
            .IsUnique()
            .HasFilter("tx_id IS NOT NULL");  // índice parcial (só onde tem txid)

        // Status: filtros de listagem (ex: buscar todas as abertas)
        builder.HasIndex(i => i.Status)
            .HasDatabaseName("ix_invoices_status");

        // BankId + Status: reconciliação por banco
        builder.HasIndex(i => new { i.BankId, i.Status })
            .HasDatabaseName("ix_invoices_bank_status");

        // ExternalReference: busca por referência do sistema cliente
        builder.HasIndex(i => i.ExternalReference)
            .HasDatabaseName("ix_invoices_external_reference")
            .HasFilter("external_reference IS NOT NULL");

        // CreatedAt: ordenação e range queries
        builder.HasIndex(i => i.CreatedAt)
            .HasDatabaseName("ix_invoices_created_at");

        // ── Relacionamento com PixCharge ─────────────────────────────────
        // Invoice → PixCharge é um relacionamento 1:N.
        // A navegação inversa está na configuração do PixCharge.
        // Aqui apenas marcamos que Invoice é o lado "um".
        builder.HasMany<PixCharge>()
            .WithOne()
            .HasForeignKey(pc => pc.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);  // não apaga cobranças em cascata

        // ── Concorrência Otimista ────────────────────────────────────────
        // Previne atualizações simultâneas conflitantes (ex: double-pay)
        builder.Property<uint>("xmin")
            .IsRowVersion()
            .HasColumnName("xmin");
    }
}
