using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nfc.Domain.Entity;

namespace Nfc.Infra.InfraData.EF.Configurations
{
    internal class NotaFiscalConfiguration : IEntityTypeConfiguration<NotaFiscal>
    {
        public void Configure(EntityTypeBuilder<NotaFiscal> builder)
        {
            builder.ToTable("NotaFiscal");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Emissor)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(n => n.DataEmissao)
                   .IsRequired();

            builder.HasMany(n => n.Items)
                .WithOne()
                .HasForeignKey(i => i.NotaFiscalId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
