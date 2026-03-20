using Domain.Itau.Entities.Produto;

namespace Domain.Itau.Agregados.PedidoAgregado
{
    public class ItemPedido
    {
        public int Id { get; private set; }
        public int ProdutoId { get; private set; }
        public Produto? Produto { get; private set; }
        public int PedidoId { get; private set; }
        public int Quantidade { get; private set; }
        public decimal PrecoUnitario { get; private set; }
        public decimal Subtotal => Quantidade * PrecoUnitario;

        public virtual Pedido Pedido { get; set; }

        protected ItemPedido() { } // EF Core

        public ItemPedido(Produto produto, int quantidade)
        {
            ValidarProduto(produto);
            ValidarQuantidade(quantidade);

            ProdutoId = produto.Id;
            Produto = produto;
            PrecoUnitario = produto.Preco; // captura o preço no momento do pedido
            Quantidade = quantidade;
        }

        public void AtualizarQuantidade(int quantidade)
        {
            ValidarQuantidade(quantidade);
            Quantidade = quantidade;
        }

        // --- Validações privadas ---

        private static void ValidarProduto(Produto produto)
        {
            if (produto is null)
                throw new ArgumentNullException(nameof(produto), "O produto é obrigatório.");

            if (!produto.Ativo)
                throw new InvalidOperationException($"O produto '{produto.Nome}' está inativo e não pode ser adicionado ao pedido.");

            if (produto.Estoque <= 0)
                throw new InvalidOperationException($"O produto '{produto.Nome}' está sem estoque.");
        }

        private static void ValidarQuantidade(int quantidade)
        {
            if (quantidade < 1)
                throw new ArgumentException("A quantidade deve ser no mínimo 1.", nameof(quantidade));
        }
    }
}
