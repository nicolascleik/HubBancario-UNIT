using HubBancario.Domain.Aggregates.Account;
using HubBancario.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HubBancario.Infrastructure.Persistence.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            // Define o nome oficial da tabela no PostgreSQL
            builder.ToTable("accounts");

            // Chave Primária
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.SecretId)
                .HasColumnName("secret_id")
                .IsRequired();

            // Regra de Ouro do DDD: Converte o Value Object Document em uma string simples na tabela
            builder.Property(x => x.Document)
                .HasConversion(
                    doc => doc.Value,
                    value => Document.Create(value))
                .HasColumnName("document")
                .HasMaxLength(14)
                .IsRequired();

            builder.Property(x => x.BankId)
                .HasColumnName("bank_id")
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.AccountNumber)
                .HasColumnName("account_number")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Agency)
                .HasColumnName("agency")
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true)
                .IsRequired();

            // Configuração do relacionamento lógico/físico 1:1 com as credenciais
            builder.HasOne<ClientSecret>()
                .WithMany()
                .HasForeignKey(x => x.SecretId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}