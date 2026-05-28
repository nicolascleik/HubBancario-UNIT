using HubBancario.Domain.Aggregates.Account;
using HubBancario.Domain.Aggregates.Audit;
using HubBancario.Domain.Aggregates.Invoice;
using HubBancario.Domain.Aggregates.PixCharge;
using Microsoft.EntityFrameworkCore;

namespace HubBancario.Infrastructure.Persistence
{
    public class BankingDbContext : DbContext
    {
        public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options)
        {
        }

        // DbSets que representam as tabelas do Hub no PostgreSQL
        public DbSet<Account> Accounts { get; set; }
        public DbSet<PixKey> PixKeys { get; set; }
        public DbSet<ClientSecret> ClientSecrets { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<PixCharge> PixCharges { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Escaneia automaticamente o assembly atual procurando e registrando
            // todas as configurações que implementam IEntityTypeConfiguration<>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankingDbContext).Assembly);
        }
    }
}