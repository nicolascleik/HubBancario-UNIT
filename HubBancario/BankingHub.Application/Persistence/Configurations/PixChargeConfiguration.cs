using BankingHub.Domain.Aggregates.Invoice;
using BankingHub.Domain.Aggregates.PixCharge;
using BankingHub.Domain.ValueObjects;
using BankingHub.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace BankingHub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração EF Core para o Aggregate Root PixCharge.
/// Uma PixCharge representa a cobrança criada no banco (Cob ou CobV),
/// vinculada a uma Invoice via InvoiceId.
/// </summary>
public sealed class PixChargeConfiguration : IEntityTypeConfiguration<PixCharge>
{
    public void Configure(EntityTypeBuilder<PixCharge> builder)
    {
        // ── Tabela ───────────────────────────────────────────────────────
        builder.ToTable("pix_charges");

        // ── Chave Primária (Strongly Typed ID) ───────────────────────────
        builder.HasKey(pc => pc.Id);

        builder.Property(pc => pc.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => ChargeId.From(value))
            .IsRequired();

        // ── Value Object: TxId ───────────────────────────────────────────
        // TxId é a chave universal de conciliação do Pix (até 35 chars alfanum.)
        builder.Property(pc => pc.TxId)
            .HasColumnName("tx_id")
            .HasConversion(
                txId => txId.Value,
                value => TxId.From(value))
            .HasMaxLength(35)
            .IsRequired();

        // ── FK para Invoice ──────────────────────────────────────────────
        builder.Property(pc => pc.InvoiceId)
            .HasColumnName("invoice_id")
            .HasConversion(
                id => id.Value,
                value => InvoiceId.From(value))
            .IsRequired();

        // ── Value Object: EmvCode ────────────────────────────────────────
        // Código "Pix Copia e Cola" no padrão EMV. Pode ter até ~500 chars.
        builder.Property(pc => pc.Emv)
            .HasColumnName("emv")
            .HasConversion(
                emv => emv.Value,
                value => EmvCode.From(value))
            .HasMaxLength(512)
            .IsRequired();

        // ── Value Object: Money (PaidAmount) ─────────────────────────────
        // Preenchido apenas quando o pagamento é confirmado.
        builder.OwnsOne(pc => pc.PaidAmount, money =>
        {
            money.Property(m => m.Value)
                .HasColumnName("paid_amount")
                .HasColumnType("numeric(18,2)")
                .IsRequired(false);

            money.Property(m => m.Currency)
                .HasColumnName("paid_currency")
                .HasMaxLength(3)
                .IsRequired(false);
        });

        // ── Propriedades Simples ─────────────────────────────────────────
        builder.Property(pc => pc.BankId)
            .HasColumnName("bank_id")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(pc => pc.ChargeType)
            .HasColumnName("charge_type")
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(pc => pc.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(pc => pc.PixLink)
            .HasColumnName("pix_link")
            .HasMaxLength(512)
            .IsRequired(false);

        builder.Property(pc => pc.PaymentId)
            .HasColumnName("payment_id")   // endToEndId retornado pelo banco
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(pc => pc.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(pc => pc.PaidAt)
            .HasColumnName("paid_at")
            .IsRequired(false);

        builder.Property(pc => pc.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired(false);

        // ── RawPayload (payload bruto do banco) ──────────────────────────
        // REGRA 3.1.5: todos os payloads brutos devem ser armazenados para auditoria.
        // Serializado como JSONB para permitir queries ad-hoc no futuro.
        builder.Property(pc => pc.RawPayload)
            .HasColumnName("raw_payload")
            .HasColumnType("jsonb")
            .HasConversion(
                obj => JsonSerializer.Serialize(obj, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<object>(json, (JsonSerializerOptions?)null)!)
            .IsRequired(false);

        // ── Índices ──────────────────────────────────────────────────────
        // TxId: chave de conciliação primária — deve ser único
        builder.HasIndex(pc => pc.TxId)
            .HasDatabaseName("ix_pix_charges_tx_id")
            .IsUnique();

        // InvoiceId: busca de cobranças por fatura
        builder.HasIndex(pc => pc.InvoiceId)
            .HasDatabaseName("ix_pix_charges_invoice_id");

        // Status: reconciliação (buscar todas ACTIVE para polling)
        builder.HasIndex(pc => pc.Status)
            .HasDatabaseName("ix_pix_charges_status");

        // BankId + Status: polling por banco
        builder.HasIndex(pc => new { pc.BankId, pc.Status })
            .HasDatabaseName("ix_pix_charges_bank_status");

        // CreatedAt: range queries e reconciliação temporal
        builder.HasIndex(pc => pc.CreatedAt)
            .HasDatabaseName("ix_pix_charges_created_at");

        // ExpiresAt: PollingJob usa esse índice para buscar cobranças próximas de expirar
        builder.HasIndex(pc => pc.ExpiresAt)
            .HasDatabaseName("ix_pix_charges_expires_at")
            .HasFilter("expires_at IS NOT NULL AND status = 'Active'");
    }
}
