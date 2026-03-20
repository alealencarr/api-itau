using Domain.Itau.Agregados.PedidoAgregado;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Itau.EntitiesConfigurations
{
    public class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
    {
        public void Configure(EntityTypeBuilder<ItemPedido> entity)
        {
            entity.ToTable("ItensPedido");

            entity.HasKey(i => i.Id);

            entity.Property(i => i.Id)
                .ValueGeneratedOnAdd();

            entity.Property(i => i.ProdutoId)
                .IsRequired();

            entity.Property(i => i.Quantidade)
                .IsRequired();

            entity.Property(i => i.PrecoUnitario)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // Subtotal é calculado (Quantidade * PrecoUnitario), não persiste no banco
            entity.Ignore(i => i.Subtotal);

            entity.HasOne(i => i.Produto)
                  .WithMany()
                  .HasForeignKey(i => i.ProdutoId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
