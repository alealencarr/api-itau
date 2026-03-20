using System.ComponentModel.DataAnnotations;

namespace Application.Itau.Pedidos.Dtos.Requests
{
    public record PedidoRequestDto
    {
        [Required(ErrorMessage = "O número do pedido é obrigatório.")]
        public string NumeroPedido { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        [MaxLength(150, ErrorMessage = "O nome do cliente não pode ultrapassar 150 caracteres.")]
        public string ClienteNome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail do cliente é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        public string ClienteEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "O pedido deve conter ao menos um item.")]
        [MinLength(1, ErrorMessage = "O pedido deve conter ao menos um item.")]
        public List<ItemPedidoRequestDto> Itens { get; set; } = new();
    }
}
