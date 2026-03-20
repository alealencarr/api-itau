using Application.Itau.Pedidos.Dtos.Requests;
using Application.Itau.Pedidos.Services;
using Domain.Itau.Agregados.PedidoAgregado;
using Domain.Itau.Agregados.PedidoAgregado.Repositories;
using Domain.Itau.Entities.Produto;
using Domain.Itau.Entities.Produto.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Net;

namespace Tests.Itau.Application;
public class PedidosServiceTests
{
    private readonly IPedidosRepository _pedidosRepository;
    private readonly IProdutosRepository _produtosRepository;
    private readonly ILogger<PedidosService> _logger;
    private readonly PedidosService _service;

    public PedidosServiceTests()
    {
        _pedidosRepository = Substitute.For<IPedidosRepository>();
        _produtosRepository = Substitute.For<IProdutosRepository>();
        _logger = Substitute.For<ILogger<PedidosService>>();
        _service = new PedidosService(_pedidosRepository, _logger, _produtosRepository);
    }

 
    private static Produto CriarProduto(int id = 1, decimal preco = 1000m, int estoque = 10)
    {
        var produto = new Produto("Produto", preco, estoque: estoque);
        typeof(Produto).GetProperty("Id")!.SetValue(produto, id);
        return produto;
    }

    private static Pedido CriarPedido(string numero = "PED-001")
    {
        var itens = new List<ItemPedido> { new ItemPedido(CriarProduto(), 1) };
        return new Pedido(numero, "João Silva", "joao@email.com", itens);
    }

    private static PedidoRequestDto CriarPedidoDto(
        string numero = "PED-001",
        string email = "joao@email.com",
        List<ItemPedidoRequestDto>? itens = null)
        => new PedidoRequestDto
        {
            NumeroPedido = numero,
            ClienteNome = "João Silva",
            ClienteEmail = email,
            Itens = itens ?? new List<ItemPedidoRequestDto>
            {
                    new ItemPedidoRequestDto { ProdutoId = 1, Quantidade = 2 }
            }
        };

 
    [Fact]
    public async Task Create_DeveRetornar201_QuandoDadosValidos()
    {
        _produtosRepository.GetPorIds(Arg.Any<HashSet<int>>()).Returns(new List<Produto> { CriarProduto(id: 1) });
        _pedidosRepository.Create(Arg.Any<Pedido>()).Returns(call => call.Arg<Pedido>());

        var result = await _service.Create(CriarPedidoDto());

        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_DeveRetornar404_QuandoProdutoNaoExiste()
    {
        _produtosRepository.GetPorIds(Arg.Any<HashSet<int>>()).Returns(new List<Produto>());

        var result = await _service.Create(CriarPedidoDto());

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await _pedidosRepository.DidNotReceive().Create(Arg.Any<Pedido>());
    }

    [Fact]
    public async Task Create_DeveRetornar404_QuandoAlgumProdutoNaoEncontrado()
    {
        // Apenas produto 1 existe, produto 2 não
        _produtosRepository.GetPorIds(Arg.Any<HashSet<int>>()).Returns(new List<Produto> { CriarProduto(id: 1) });

        var dto = CriarPedidoDto(itens: new List<ItemPedidoRequestDto>
            {
                new ItemPedidoRequestDto { ProdutoId = 1, Quantidade = 1 },
                new ItemPedidoRequestDto { ProdutoId = 2, Quantidade = 1 }
            });

        var result = await _service.Create(dto);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_DeveRetornar400_QuandoProdutoInativo()
    {
        var produto = CriarProduto(id: 1);
        produto.Desativar();
        _produtosRepository.GetPorIds(Arg.Any<HashSet<int>>()).Returns(new List<Produto> { produto });

        var result = await _service.Create(CriarPedidoDto());

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await _pedidosRepository.DidNotReceive().Create(Arg.Any<Pedido>());
    }

    [Fact]
    public async Task Create_DeveRetornar400_QuandoEmailInvalido()
    {
        var result = await _service.Create(CriarPedidoDto(email: "email-invalido"));

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_DeveAgruparQuantidades_QuandoMesmoProdutoRepetido()
    {
        var produto = CriarProduto(id: 1);
        _produtosRepository.GetPorIds(Arg.Any<HashSet<int>>()).Returns(new List<Produto> { produto });
        _pedidosRepository.Create(Arg.Any<Pedido>()).Returns(call => call.Arg<Pedido>());

        var dto = CriarPedidoDto(itens: new List<ItemPedidoRequestDto>
            {
                new ItemPedidoRequestDto { ProdutoId = 1, Quantidade = 2 },
                new ItemPedidoRequestDto { ProdutoId = 1, Quantidade = 3 }
            });

        var result = await _service.Create(dto);

        result.StatusCode.Should().Be(HttpStatusCode.Created);
        await _pedidosRepository.Received(1).Create(
            Arg.Is<Pedido>(p => p.Itens.Count == 1 && p.Itens.First().Quantidade == 5));
    }

    [Fact]
    public async Task Create_DeveRetornar500_QuandoRepositoryLancaException()
    {
        _produtosRepository.GetPorIds(Arg.Any<HashSet<int>>()).Returns(new List<Produto> { CriarProduto(id: 1) });
        _pedidosRepository.Create(Arg.Any<Pedido>()).Throws(new Exception("DB error"));

        var result = await _service.Create(CriarPedidoDto());

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
 

    [Fact]
    public async Task GetAll_DeveRetornar200_QuandoExistemPedidos()
    {
        _pedidosRepository.GetAll().Returns(new List<Pedido> { CriarPedido("PED-001"), CriarPedido("PED-002") });

        var result = await _service.GetAll();

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Data.Should().HaveCount(2);
    }

 
    [Fact]
    public async Task GetAll_DeveRetornar500_QuandoRepositoryLancaException()
    {
        _pedidosRepository.GetAll().Throws(new Exception("DB error"));

        var result = await _service.GetAll();

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

  

    [Fact]
    public async Task GetPorId_DeveRetornar200_QuandoPedidoExiste()
    {
        _pedidosRepository.GetPorId(1).Returns(CriarPedido());

        var result = await _service.GetPorId(1);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPorId_DeveRetornar404_QuandoPedidoNaoExiste()
    {
        _pedidosRepository.GetPorId(999).Returns((Pedido?)null);

        var result = await _service.GetPorId(999);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetPorId_DeveMapearNumeroPedido_NoDto()
    {
        _pedidosRepository.GetPorId(1).Returns(CriarPedido("PED-999"));

        var result = await _service.GetPorId(1);

        result.Data!.NumeroPedido.Should().Be("PED-999");
    }

    [Fact]
    public async Task GetPorId_DeveRetornar500_QuandoRepositoryLancaException()
    {
        _pedidosRepository.GetPorId(Arg.Any<int>()).Throws(new Exception("DB error"));

        var result = await _service.GetPorId(1);

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
 

    [Fact]
    public async Task UpdateStatus_DeveRetornar204_QuandoTransicaoValida()
    {
        _pedidosRepository.GetPorId(1).Returns(CriarPedido());
        _pedidosRepository.Update(Arg.Any<Pedido>()).Returns(Task.CompletedTask);

        var result = await _service.UpdateStatus(1, new PedidoStatusRequestDto { Status = "Processando" });

        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateStatus_DeveRetornar404_QuandoPedidoNaoExiste()
    {
        _pedidosRepository.GetPorId(999).Returns((Pedido?)null);

        var result = await _service.UpdateStatus(999, new PedidoStatusRequestDto { Status = "Processando" });

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await _pedidosRepository.DidNotReceive().Update(Arg.Any<Pedido>());
    }

    [Fact]
    public async Task UpdateStatus_DeveRetornar400_QuandoStatusInexistente()
    {
        _pedidosRepository.GetPorId(1).Returns(CriarPedido());

        var result = await _service.UpdateStatus(1, new PedidoStatusRequestDto { Status = "StatusInexistente" });

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateStatus_DeveRetornar400_QuandoTransicaoInvalida_PendenteParaEntregue()
    {
        _pedidosRepository.GetPorId(1).Returns(CriarPedido()); // status = Pendente

        var result = await _service.UpdateStatus(1, new PedidoStatusRequestDto { Status = "Entregue" });

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await _pedidosRepository.DidNotReceive().Update(Arg.Any<Pedido>());
    }

    [Fact]
    public async Task UpdateStatus_DeveRetornar400_QuandoPedidoJaEntregue()
    {
        var pedido = CriarPedido();
        pedido.AvancarStatus(Status.Processando);
        pedido.AvancarStatus(Status.Enviado);
        pedido.AvancarStatus(Status.Entregue);
        _pedidosRepository.GetPorId(1).Returns(pedido);

        var result = await _service.UpdateStatus(1, new PedidoStatusRequestDto { Status = "Cancelado" });

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateStatus_DeveRetornar500_QuandoRepositoryLancaException()
    {
        _pedidosRepository.GetPorId(1).Returns(CriarPedido());
        _pedidosRepository.Update(Arg.Any<Pedido>()).Throws(new Exception("DB error"));

        var result = await _service.UpdateStatus(1, new PedidoStatusRequestDto { Status = "Processando" });

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
 
    [Fact]
    public async Task Delete_DeveRetornar204_QuandoPedidoExiste()
    {
        _pedidosRepository.GetPorId(1).Returns(CriarPedido());
        _pedidosRepository.Update(Arg.Any<Pedido>()).Returns(Task.CompletedTask);

        var result = await _service.Delete(1);

        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_DeveCancelarPedido_AoInvesDeExcluirFisicamente()
    {
        var pedido = CriarPedido();
        _pedidosRepository.GetPorId(1).Returns(pedido);

        await _service.Delete(1);

        pedido.Status.Should().Be(Status.Cancelado);
        await _pedidosRepository.Received(1).Update(
            Arg.Is<Pedido>(p => p.Status == Status.Cancelado));
    }

    [Fact]
    public async Task Delete_DeveRetornar404_QuandoPedidoNaoExiste()
    {
        _pedidosRepository.GetPorId(999).Returns((Pedido?)null);

        var result = await _service.Delete(999);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await _pedidosRepository.DidNotReceive().Update(Arg.Any<Pedido>());
    }

    [Fact]
    public async Task Delete_DeveRetornar500_QuandoRepositoryLancaException()
    {
        _pedidosRepository.GetPorId(1).Returns(CriarPedido());
        _pedidosRepository.Update(Arg.Any<Pedido>()).Throws(new Exception("DB error"));

        var result = await _service.Delete(1);

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}