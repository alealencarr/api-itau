using System.ComponentModel.DataAnnotations;

namespace Application.Itau.Produtos.Dtos.Requests
{
    public class ProdutoRequestDto
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome não pode ultrapassar 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "A descrição não pode ultrapassar 500 caracteres.")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O preço do produto é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo.")]
        public int Estoque { get; set; }
    }
}
