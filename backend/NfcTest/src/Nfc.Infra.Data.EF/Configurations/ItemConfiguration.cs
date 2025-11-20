using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nfc.Domain.Entity;

namespace Nfc.Infra.InfraData.EF.Configurations
{
    internal class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Item");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Descricao)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(i => i.Valor)
                   .HasColumnType("decimal(15,3)")
                   .IsRequired();

            builder.HasOne<NotaFiscal>()
                   .WithMany(n => n.Items)
                   .HasForeignKey(i => i.NotaFiscalId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
