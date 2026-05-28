using HubBancario.Domain.Aggregates.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HubBancario.Infrastructure.Persistence.Configurations
{
    public class ClientSecretConfiguration : IEntityTypeConfiguration<ClientSecret>
    {
        public void Configure(EntityTypeBuilder<ClientSecret> builder)
        {
            builder.ToTable("client_secrets");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnName("id");

            // Mapeia a propriedade 'ClientId' do domínio para a coluna 'account_id' do PostgreSQL
            builder.Property(x => x.ClientId)
                .HasColumnName("account_id")
                .IsRequired();

            builder.Property(x => x.SecretValue)
                .HasColumnName("secret_value")
                .HasMaxLength(255)
                .IsRequired();

            // Tipo 'text' no PostgreSQL suporta sem problemas strings imensas como chaves PEM ou certificados em Base64
            builder.Property(x => x.Certificate)
                .HasColumnName("certificate")
                .HasColumnType("text")
                .IsRequired();

            builder.Property(x => x.CertificatePassword)
                .HasColumnName("certificate_password")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.IsValid)
                .HasColumnName("is_valid")
                .HasDefaultValue(true)
                .IsRequired();

            // Relacionamento: O segredo pertence a uma conta cadastrada no Hub
            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(x => x.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}