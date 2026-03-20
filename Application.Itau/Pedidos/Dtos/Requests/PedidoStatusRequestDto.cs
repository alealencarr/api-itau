using System.ComponentModel.DataAnnotations;

namespace Application.Itau.Pedidos.Dtos.Requests
{
    public class PedidoStatusRequestDto
    {
        [Required(ErrorMessage = "É necessário informar o novo status.")]
        public string Status { get; set; } = null!;
    }
}
