using HubBancario.Domain.Aggregates.Invoice;
using HubBancario.Domain.Aggregates.PixCharge;
using HubBancario.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HubBancario.Infrastructure.Persistence.Configurations
{
    public class PixChargeConfiguration : IEntityTypeConfiguration<PixCharge>
    {
        public void Configure(EntityTypeBuilder<PixCharge> builder)
        {
            builder.ToTable("pix_charges");

            // A Chave Primária é o Value Object TxId convertido para string!
            builder.HasKey(x => x.TxId);
            builder.Property(x => x.TxId)
                .HasConversion(
                    txId => txId.Value, 
                    value => TxId.From(value))
                .HasColumnName("tx_id")
                .HasMaxLength(35) // Trava máxima do BACEN
                .IsRequired();

            builder.Property(x => x.InvoiceId)
                .HasColumnName("invoice_id")
                .IsRequired();

            builder.Property(x => x.ChargeType)
                .HasConversion<string>()
                .HasColumnName("charge_type")
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasColumnName("status")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Emv)
                .HasColumnName("emv")
                .HasColumnType("text")
                .IsRequired();

            builder.Property(x => x.RawPayload)
                .HasColumnName("raw_payload")
                .HasColumnType("jsonb"); // Guarda o JSON do banco parceiro em formato nativo

            // Relacionamento 1:N com Invoice
            builder.HasOne<Invoice>()
                .WithMany()
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}