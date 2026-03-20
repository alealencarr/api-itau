using Domain.Itau.Entities.Produto;

namespace Application.Itau.Pedidos.Models
{
    public class ItemPedidoConsolidadoModel
    {
        public Produto Produto { get; set; } = default!;
        public int Quantidade { get; set; }
    }
}
