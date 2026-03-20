using Domain.Itau.Entities.Produto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Itau.EntitiesConfigurations
{
    public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> entity)
        {
            entity.ToTable("Produtos");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            entity.Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.Descricao)
                .HasMaxLength(500)
                .IsRequired(false);

            entity.Property(p => p.Preco)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(p => p.Estoque)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(p => p.DataCriacao)
                .IsRequired();

            entity.Property(p => p.Ativo)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}


 