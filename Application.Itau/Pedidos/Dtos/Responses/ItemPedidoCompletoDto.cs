using Application.Itau.Produtos.Dtos.Responses;

namespace Application.Itau.Pedidos.Dtos.Responses
{
    public record ItemPedidoCompletoDto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}