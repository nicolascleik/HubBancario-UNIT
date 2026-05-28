using HubBancario.Domain.Aggregates.Account;
using HubBancario.Domain.Aggregates.Invoice;
using HubBancario.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HubBancario.Infrastructure.Persistence.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("invoices");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.AccountId)
                .HasColumnName("account_id")
                .IsRequired();

            // Conversão de Value Object para o Banco de Dados (Desconstrói para gravar, Constrói para ler)
            builder.Property(x => x.Amount)
                .HasConversion(
                    money => money.Value, 
                    value => Money.BRL(value))
                .HasColumnName("amount")
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            builder.Property(x => x.DueDate)
                .HasColumnName("due_date")
                .IsRequired();

            // Salva o Enum como String no banco (ex: "Open", "Paid") para facilitar a leitura humana no banco
            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasColumnName("status")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.ExternalReference)
                .HasColumnName("external_reference")
                .HasMaxLength(100)
                .IsRequired();

            // Relacionamento: Fatura pertence a uma Conta
            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict); // Não permite apagar uma conta se houver faturas atreladas
        }
    }
}