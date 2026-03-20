namespace Application.Itau.Pedidos.Dtos.Responses
{
    public record PedidoCompletoDto
    {
        public int Id { get; set; }
        public string NumeroPedido { get; set; } = string.Empty;
        public string ClienteNome { get; set; } = string.Empty;
        public string ClienteEmail { get; set; } = string.Empty;
        public DateTime DataPedido { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public List<ItemPedidoCompletoDto> Itens { get; set; } = new();
    }
}
