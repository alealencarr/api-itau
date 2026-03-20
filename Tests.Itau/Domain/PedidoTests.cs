using Domain.Itau.Agregados.PedidoAgregado;
using Domain.Itau.Entities.Produto;
using FluentAssertions;

namespace Tests.Itau.Domain
{
    public class PedidoTests
    { 
        private static Produto CriarProdutoValido(string nome = "Notebook", decimal preco = 1000m, int estoque = 10)
            => new Produto(nome, preco, estoque: estoque);

        private static ItemPedido CriarItemValido(Produto? produto = null, int quantidade = 1)
            => new ItemPedido(produto ?? CriarProdutoValido(), quantidade);

        private static Pedido CriarPedidoValido(List<ItemPedido>? itens = null)
            => new Pedido("PED-001", "João Silva", "joao@email.com",
                itens ?? new List<ItemPedido> { CriarItemValido() });

 

        [Fact]
        public void Construtor_DeveCriarPedido_QuandoDadosValidos()
        {
            var item = CriarItemValido();
            var pedido = new Pedido("PED-001", "João Silva", "joao@email.com", new List<ItemPedido> { item });

            pedido.NumeroPedido.Should().Be("PED-001");
            pedido.ClienteNome.Should().Be("João Silva");
            pedido.ClienteEmail.Should().Be("joao@email.com");
            pedido.Status.Should().Be(Status.Pendente);
            pedido.DataPedido.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            pedido.Itens.Should().HaveCount(1);
        }

        [Fact]
        public void Construtor_DeveFazerNormalizeNoEmail()
        {
            var pedido = new Pedido("PED-001", "João", "JOAO@EMAIL.COM", new List<ItemPedido> { CriarItemValido() });
            pedido.ClienteEmail.Should().Be("joao@email.com");
        }

        [Fact]
        public void Construtor_DeveCalcularValorTotal_ComBaseNosItens()
        {
            var produto = CriarProdutoValido(preco: 500m);
            var itens = new List<ItemPedido>
            {
                new ItemPedido(produto, 2), // 2 x 500 = 1000
                new ItemPedido(produto, 3)  // 3 x 500 = 1500
            };

            var pedido = new Pedido("PED-001", "João", "joao@email.com", itens);

            pedido.ValorTotal.Should().Be(2500m);
        }

 

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_DeveLancarArgumentException_QuandoNumeroPedidoVazioOuNulo(string numero)
        {
            var act = () => new Pedido(numero, "João", "joao@email.com", new List<ItemPedido> { CriarItemValido() });
            act.Should().Throw<ArgumentException>().WithParameterName("numeroPedido");
        }
 

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_DeveLancarArgumentException_QuandoClienteNomeVazioOuNulo(string nome)
        {
            var act = () => new Pedido("PED-001", nome, "joao@email.com", new List<ItemPedido> { CriarItemValido() });
            act.Should().Throw<ArgumentException>().WithParameterName("clienteNome");
        }

        [Fact]
        public void Construtor_DeveLancarArgumentException_QuandoClienteNomeAcimaDeMaxLength()
        {
            var nomeInvalido = new string('A', 151);
            var act = () => new Pedido("PED-001", nomeInvalido, "joao@email.com", new List<ItemPedido> { CriarItemValido() });
            act.Should().Throw<ArgumentException>().WithParameterName("clienteNome");
        }

        [Fact]
        public void Construtor_DeveAceitar_QuandoClienteNomeExatamente150Caracteres()
        {
            var nome = new string('A', 150);
            var act = () => new Pedido("PED-001", nome, "joao@email.com", new List<ItemPedido> { CriarItemValido() });
            act.Should().NotThrow();
        }
 

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_DeveLancarArgumentException_QuandoEmailVazioOuNulo(string email)
        {
            var act = () => new Pedido("PED-001", "João", email, new List<ItemPedido> { CriarItemValido() });
            act.Should().Throw<ArgumentException>().WithParameterName("email");
        }

        [Theory]
        [InlineData("email-invalido")]
        [InlineData("@dominio.com")]
        [InlineData("email@")]
        [InlineData("semdominio")]
        public void Construtor_DeveLancarArgumentException_QuandoEmailInvalido(string email)
        {
            var act = () => new Pedido("PED-001", "João", email, new List<ItemPedido> { CriarItemValido() });
            act.Should().Throw<ArgumentException>().WithParameterName("email");
        }

        [Theory]
        [InlineData("joao@email.com")]
        [InlineData("joao.silva@empresa.com.br")]
        [InlineData("usuario+tag@dominio.org")]
        public void Construtor_DeveAceitar_QuandoEmailValido(string email)
        {
            var act = () => new Pedido("PED-001", "João", email, new List<ItemPedido> { CriarItemValido() });
            act.Should().NotThrow();
        }

 
        [Fact]
        public void Construtor_DeveLancarArgumentException_QuandoItensNulos()
        {
            var act = () => new Pedido("PED-001", "João", "joao@email.com", null!);
            act.Should().Throw<ArgumentException>().WithParameterName("itens");
        }

        [Fact]
        public void Construtor_DeveLancarArgumentException_QuandoItensVazios()
        {
            var act = () => new Pedido("PED-001", "João", "joao@email.com", new List<ItemPedido>());
            act.Should().Throw<ArgumentException>().WithParameterName("itens");
        }

 

        [Fact]
        public void AvancarStatus_DeveAvancar_QuandoPendenteParaProcessando()
        {
            var pedido = CriarPedidoValido();
            pedido.AvancarStatus(Status.Processando);
            pedido.Status.Should().Be(Status.Processando);
        }

        [Fact]
        public void AvancarStatus_DeveAvancar_QuandoProcessandoParaEnviado()
        {
            var pedido = CriarPedidoValido();
            pedido.AvancarStatus(Status.Processando);
            pedido.AvancarStatus(Status.Enviado);
            pedido.Status.Should().Be(Status.Enviado);
        }

        [Fact]
        public void AvancarStatus_DeveAvancar_QuandoEnviadoParaEntregue()
        {
            var pedido = CriarPedidoValido();
            pedido.AvancarStatus(Status.Processando);
            pedido.AvancarStatus(Status.Enviado);
            pedido.AvancarStatus(Status.Entregue);
            pedido.Status.Should().Be(Status.Entregue);
        }

        [Fact]
        public void AvancarStatus_DeveCancelar_QuandoPendenteParaCancelado()
        {
            var pedido = CriarPedidoValido();
            pedido.AvancarStatus(Status.Cancelado);
            pedido.Status.Should().Be(Status.Cancelado);
        }

        [Fact]
        public void AvancarStatus_DeveCancelar_QuandoProcessandoParaCancelado()
        {
            var pedido = CriarPedidoValido();
            pedido.AvancarStatus(Status.Processando);
            pedido.AvancarStatus(Status.Cancelado);
            pedido.Status.Should().Be(Status.Cancelado);
        }

   

        [Fact]
        public void AvancarStatus_DeveLancarArgumentException_QuandoStatusInexistente()
        {
            var pedido = CriarPedidoValido();
            var act = () => pedido.AvancarStatus("StatusInexistente");
            act.Should().Throw<ArgumentException>().WithParameterName("novoStatus");
        }

        [Fact]
        public void AvancarStatus_DeveLancarInvalidOperationException_QuandoPendenteParaEnviado()
        {
            var pedido = CriarPedidoValido();
            var act = () => pedido.AvancarStatus(Status.Enviado);
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void AvancarStatus_DeveLancarInvalidOperationException_QuandoPendenteParaEntregue()
        {
            var pedido = CriarPedidoValido();
            var act = () => pedido.AvancarStatus(Status.Entregue);
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void AvancarStatus_DeveLancarInvalidOperationException_QuandoPedidoJaEntregue()
        {
            var pedido = CriarPedidoValido();
            pedido.AvancarStatus(Status.Processando);
            pedido.AvancarStatus(Status.Enviado);
            pedido.AvancarStatus(Status.Entregue);

            var act = () => pedido.AvancarStatus(Status.Cancelado);
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void AvancarStatus_DeveLancarInvalidOperationException_QuandoPedidoJaCancelado()
        {
            var pedido = CriarPedidoValido();
            pedido.AvancarStatus(Status.Cancelado);

            var act = () => pedido.AvancarStatus(Status.Processando);
            act.Should().Throw<InvalidOperationException>();
        }

      

        [Fact]
        public void CancelarPedido_DeveDefinirStatusComoCancelado_QuandoPedidoPendente()
        {
            var pedido = CriarPedidoValido();
            pedido.CancelarPedido();
            pedido.Status.Should().Be(Status.Cancelado);
        }

        [Fact]
        public void ValorTotal_DeveSerSomaDosSubtotaisDoItens()
        {
            var produto = CriarProdutoValido(preco: 200m);
            var itens = new List<ItemPedido>
            {
                new ItemPedido(produto, 1), // 200
                new ItemPedido(produto, 4)  // 800
            };
            var pedido = new Pedido("PED-001", "João", "joao@email.com", itens);
            pedido.ValorTotal.Should().Be(1000m);
        }
    }

    public class ItemPedidoTests
    {
        private static Produto CriarProdutoValido(decimal preco = 100m, int estoque = 10)
            => new Produto("Produto", preco, estoque: estoque);

        [Fact]
        public void Construtor_DeveCriarItem_QuandoDadosValidos()
        {
            var produto = CriarProdutoValido(preco: 250m);
            var item = new ItemPedido(produto, 2);

            item.ProdutoId.Should().Be(produto.Id);
            item.Quantidade.Should().Be(2);
            item.PrecoUnitario.Should().Be(250m);
            item.Subtotal.Should().Be(500m);
        }

        [Fact]
        public void Construtor_DeveCapturarPrecoNoMomentoDoItem()
        {
            var produto = CriarProdutoValido(preco: 150m);
            var item = new ItemPedido(produto, 1);

            produto.AtualizarPreco(999m); // altera preço após criação

            item.PrecoUnitario.Should().Be(150m); // mantém o preço original
        }

        [Fact]
        public void Subtotal_DeveSerCalculado_ComoQuantidadeVezesPrecoUnitario()
        {
            var produto = CriarProdutoValido(preco: 300m);
            var item = new ItemPedido(produto, 3);
            item.Subtotal.Should().Be(900m);
        }

        [Fact]
        public void Construtor_DeveLancarArgumentNullException_QuandoProdutoNulo()
        {
            var act = () => new ItemPedido(null!, 1);
            act.Should().Throw<ArgumentNullException>().WithParameterName("produto");
        }

        [Fact]
        public void Construtor_DeveLancarInvalidOperationException_QuandoProdutoInativo()
        {
            var produto = CriarProdutoValido();
            produto.Desativar();

            var act = () => new ItemPedido(produto, 1);
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Construtor_DeveLancarInvalidOperationException_QuandoProdutoSemEstoque()
        {
            var produto = CriarProdutoValido(estoque: 0);

            var act = () => new ItemPedido(produto, 1);
            act.Should().Throw<InvalidOperationException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Construtor_DeveLancarArgumentException_QuandoQuantidadeMenorQueUm(int quantidade)
        {
            var produto = CriarProdutoValido();
            var act = () => new ItemPedido(produto, quantidade);
            act.Should().Throw<ArgumentException>().WithParameterName("quantidade");
        }

        [Fact]
        public void AtualizarQuantidade_DeveAlterarQuantidadeERecalcularSubtotal()
        {
            var produto = CriarProdutoValido(preco: 100m);
            var item = new ItemPedido(produto, 1);

            item.AtualizarQuantidade(4);

            item.Quantidade.Should().Be(4);
            item.Subtotal.Should().Be(400m);
        }

        [Fact]
        public void AtualizarQuantidade_DeveLancarArgumentException_QuandoQuantidadeInvalida()
        {
            var produto = CriarProdutoValido();
            var item = new ItemPedido(produto, 1);
            var act = () => item.AtualizarQuantidade(0);
            act.Should().Throw<ArgumentException>().WithParameterName("quantidade");
        }
    }
}