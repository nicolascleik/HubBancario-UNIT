using HubBancario.Domain.Aggregates.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HubBancario.Infrastructure.Persistence.Configurations
{
    public class PixKeyConfiguration : IEntityTypeConfiguration<PixKey>
    {
        public void Configure(EntityTypeBuilder<PixKey> builder)
        {
            builder.ToTable("pix_keys");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.KeyValue)
                .HasColumnName("key_value")
                .HasMaxLength(77) // Tamanho máximo recomendado para chaves tipo E-mail ou Aleatória (EVP)
                .IsRequired();

            builder.Property(x => x.AccountId)
                .HasColumnName("account_id")
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true)
                .IsRequired();

            // Relacionamento de Chaves: Muitas chaves Pix pertencem a uma única Conta cadastrada
            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Criação de índices estratégicos para consultas rápidas no banco
            builder.HasIndex(x => x.AccountId);
            builder.HasIndex(x => x.KeyValue);
        }
    }
}