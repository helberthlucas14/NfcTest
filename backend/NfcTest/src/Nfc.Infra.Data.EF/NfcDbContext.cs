using Microsoft.EntityFrameworkCore;
using Nfc.Domain.Entity;


namespace Nfc.Infra.Data.EF
{
    public class NfcDbContext : DbContext
    {
        public NfcDbContext(DbContextOptions<NfcDbContext> options)
            : base(options) { }
        public DbSet<NotaFiscal> NotasFiscais => Set<NotaFiscal>();
        public DbSet<Item> Itens => Set<Item>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(NfcDbContext).Assembly);
        }
    }
}
