using System.ComponentModel.DataAnnotations;

namespace Application.Itau.Pedidos.Dtos.Requests
{
    public record ItemPedidoRequestDto
    {
        [Required(ErrorMessage = "O ProdutoId é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O ProdutoId deve ser um valor válido.")]
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser no mínimo 1.")]
        public int Quantidade { get; set; }
    }
}
