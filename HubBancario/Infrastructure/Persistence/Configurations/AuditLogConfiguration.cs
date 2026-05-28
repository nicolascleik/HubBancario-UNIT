using HubBancario.Domain.Aggregates.Account;
using HubBancario.Domain.Aggregates.Audit;
using HubBancario.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HubBancario.Infrastructure.Persistence.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("audit_logs");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.AccountId)
                .HasColumnName("account_id")
                .IsRequired();

            builder.Property(x => x.OccurredAt)
                .HasColumnName("occurred_at")
                .IsRequired();

            builder.Property(x => x.PaymentStatus)
                .HasColumnName("payment_status")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.TxId)
                .HasConversion(
                    txId => txId.Value, 
                    value => TxId.From(value))
                .HasColumnName("tx_id")
                .HasMaxLength(35)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasConversion(
                    money => money.Value, 
                    value => Money.BRL(value))
                .HasColumnName("amount")
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            // Mapeamento otimizado para o PostgreSQL: jsonb nativo
            builder.Property(x => x.PayloadDetails)
                .HasColumnName("payload_details")
                .HasColumnType("jsonb")
                .IsRequired();

            // Relacionamento
            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}