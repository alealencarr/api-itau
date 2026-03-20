using Domain.Itau.Agregados.PedidoAgregado;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Itau.EntitiesConfigurations
{
    public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> entity)
        {
            entity.ToTable("Pedidos");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            entity.Property(p => p.NumeroPedido)
                .IsRequired();

            entity.HasIndex(p => p.NumeroPedido)
                .IsUnique();

            entity.Property(p => p.ClienteNome)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(p => p.ClienteEmail)
                .IsRequired();

            entity.Property(p => p.DataPedido)
                .IsRequired();

            entity.Property(p => p.Status)
                .IsRequired()
                .HasDefaultValue("Pendente");

            // ValorTotal é calculado com base na soma dos itens, não persiste no banco
            entity.Ignore(i => i.ValorTotal);

            entity.HasMany(p => p.Itens)
                  .WithOne(i => i.Pedido)
                  .HasForeignKey(i => i.PedidoId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
