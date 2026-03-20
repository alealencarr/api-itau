using Domain.Itau.Entities.Produto;
using System.Text.RegularExpressions;

namespace Domain.Itau.Agregados.PedidoAgregado
{
    public class Pedido
    {
        public int Id { get; private set; }
        public string NumeroPedido { get; private set; }
        public string ClienteNome { get; private set; }
        public string ClienteEmail { get; private set; }
        public DateTime DataPedido { get; private set; }
        public string Status { get; private set; }
        public decimal ValorTotal => _itens.Sum(i => i.Subtotal);

        private readonly List<ItemPedido> _itens = new();
        public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

        protected Pedido() { } // EF Core

        public Pedido(string numeroPedido, string clienteNome, string clienteEmail, List<ItemPedido> itens)
        {
            ValidarNumeroPedido(numeroPedido);
            ValidarClienteNome(clienteNome);
            ValidarClienteEmail(clienteEmail);

            if (itens is null || !itens.Any())
                throw new ArgumentException("O pedido deve conter ao menos um item.", nameof(itens));

            NumeroPedido = numeroPedido.Trim();
            ClienteNome = clienteNome.Trim();
            ClienteEmail = clienteEmail.Trim().ToLowerInvariant();
            DataPedido = DateTime.UtcNow;
            Status = PedidoAgregado.Status.Pendente;
            _itens = itens;
        }

        public void AdicionarItem(Produto produto, int quantidade)
        {
            if (Status == PedidoAgregado.Status.Cancelado)
                throw new InvalidOperationException("Não é possível adicionar itens a um pedido cancelado.");

            var itemExistente = _itens.FirstOrDefault(i => i.ProdutoId == produto.Id);

            if (itemExistente is not null)
                itemExistente.AtualizarQuantidade(itemExistente.Quantidade + quantidade);
            else
                _itens.Add(new ItemPedido(produto, quantidade));
        }

        public void RemoverItem(int produtoId)
        {
            var item = _itens.FirstOrDefault(i => i.ProdutoId == produtoId)
                ?? throw new InvalidOperationException($"Produto {produtoId} não encontrado nos itens do pedido.");

            _itens.Remove(item);
        }

        public void CancelarPedido()
        {
            Status = PedidoAgregado.Status.Cancelado;
        }
        public void AvancarStatus(string novoStatus)
        {
            if (!PedidoAgregado.Status.EValido(novoStatus))
                throw new ArgumentException($"Status '{novoStatus}' inválido. Valores aceitos: {string.Join(", ", new[] { PedidoAgregado.Status.Pendente, PedidoAgregado.Status.Processando, PedidoAgregado.Status.Enviado, PedidoAgregado.Status.Entregue, PedidoAgregado.Status.Cancelado })}.", nameof(novoStatus));

            if (!PedidoAgregado.Status.TransicaoPermitida(Status, novoStatus))
                throw new InvalidOperationException($"Não é permitido alterar o status de '{Status}' para '{novoStatus}'.");

            if (novoStatus != PedidoAgregado.Status.Cancelado && !_itens.Any())
                throw new InvalidOperationException("O pedido não pode avançar de status sem itens.");

            Status = novoStatus;
        }



        private static void ValidarNumeroPedido(string numeroPedido)
        {
            if (string.IsNullOrWhiteSpace(numeroPedido))
                throw new ArgumentException("O número do pedido é obrigatório.", nameof(numeroPedido));
        }

        private static void ValidarClienteNome(string clienteNome)
        {
            if (string.IsNullOrWhiteSpace(clienteNome))
                throw new ArgumentException("O nome do cliente é obrigatório.", nameof(clienteNome));

            if (clienteNome.Trim().Length > 150)
                throw new ArgumentException("O nome do cliente não pode ultrapassar 150 caracteres.", nameof(clienteNome));
        }

        private static void ValidarClienteEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("O e-mail do cliente é obrigatório.", nameof(email));

            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
            if (!regex.IsMatch(email.Trim()))
                throw new ArgumentException("O e-mail informado não é válido.", nameof(email));
        }
    }
}