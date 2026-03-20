using Application.Itau.Produtos.Dtos.Requests;
using Application.Itau.Produtos.Services;
using Domain.Itau.Agregados.PedidoAgregado;
using Domain.Itau.Entities.Produto;
using Domain.Itau.Entities.Produto.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Net;

namespace Tests.Itau.Application;
public class ProdutosServiceTests
{
    private readonly IProdutosRepository _repository;
    private readonly ILogger<ProdutosService> _logger;
    private readonly ProdutosService _service;

    public ProdutosServiceTests()
    {
        _repository = Substitute.For<IProdutosRepository>();
        _logger = Substitute.For<ILogger<ProdutosService>>();
        _service = new ProdutosService(_repository, _logger);
    }

    private static Produto CriarProduto(string nome = "Notebook", decimal preco = 999m, int estoque = 10)
        => new Produto(nome, preco, estoque: estoque);

    private static ProdutoRequestDto CriarDto(string nome = "Notebook", decimal preco = 999m, int estoque = 10, string? descricao = null)
        => new ProdutoRequestDto { Nome = nome, Preco = preco, Estoque = estoque, Descricao = descricao };
 

    [Fact]
    public async Task Create_DeveRetornar201_QuandoDadosValidos()
    {
        _repository.Create(Arg.Any<Produto>()).Returns(call => call.Arg<Produto>());

        var result = await _service.Create(CriarDto());

        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_DeveRetornar400_QuandoNomeVazio()
    {
        var result = await _service.Create(CriarDto(nome: ""));

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await _repository.DidNotReceive().Create(Arg.Any<Produto>());
    }

    [Fact]
    public async Task Create_DeveRetornar400_QuandoPrecoZero()
    {
        var result = await _service.Create(CriarDto(preco: 0));

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await _repository.DidNotReceive().Create(Arg.Any<Produto>());
    }

    [Fact]
    public async Task Create_DeveRetornar400_QuandoEstoqueNegativo()
    {
        var result = await _service.Create(CriarDto(estoque: -1));

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await _repository.DidNotReceive().Create(Arg.Any<Produto>());
    }

    [Fact]
    public async Task Create_DeveRetornar400_QuandoNomeAcimaDeMaxLength()
    {
        var result = await _service.Create(CriarDto(nome: new string('A', 101)));

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_DeveRetornar500_QuandoRepositoryLancaException()
    {
        _repository.Create(Arg.Any<Produto>()).Throws(new Exception("DB error"));

        var result = await _service.Create(CriarDto());

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
     
    [Fact]
    public async Task GetAllAtivos_DeveRetornar200_QuandoExistemProdutos()
    {
        _repository.GetAllAtivos().Returns(new List<Produto> { CriarProduto("Mouse"), CriarProduto("Teclado") });

        var result = await _service.GetAllAtivos();

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAtivos_DeveRetornar200ComListaVazia_QuandoNaoExistemProdutos()
    {
        _repository.GetAllAtivos().Returns(new List<Produto>());

        var result = await _service.GetAllAtivos();

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAtivos_DeveMapearCorretamente_ParaDto()
    {
        _repository.GetAllAtivos().Returns(new List<Produto> { CriarProduto("Notebook", 4999m, 5) });

        var result = await _service.GetAllAtivos();

        var dto = result.Data!.First();
        dto.Nome.Should().Be("Notebook");
        dto.Preco.Should().Be(4999m);
        dto.Estoque.Should().Be(5);
        dto.Ativo.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAtivos_DeveRetornar500_QuandoRepositoryLancaException()
    {
        _repository.GetAllAtivos().Throws(new Exception("DB error"));

        var result = await _service.GetAllAtivos();

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

 

    [Fact]
    public async Task GetPorId_DeveRetornar200_QuandoProdutoExiste()
    {
        _repository.GetPorId(1).Returns(CriarProduto());

        var result = await _service.GetPorId(1);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPorId_DeveRetornar404_QuandoProdutoNaoExiste()
    {
        _repository.GetPorId(999).Returns((Produto?)null);

        var result = await _service.GetPorId(999);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetPorId_DeveMapearCorretamente_ParaDto()
    {
        _repository.GetPorId(1).Returns(CriarProduto("Monitor", 1800m, 3));

        var result = await _service.GetPorId(1);

        result.Data!.Nome.Should().Be("Monitor");
        result.Data.Preco.Should().Be(1800m);
        result.Data.Estoque.Should().Be(3);
    }

    [Fact]
    public async Task GetPorId_DeveRetornar500_QuandoRepositoryLancaException()
    {
        _repository.GetPorId(Arg.Any<int>()).Throws(new Exception("DB error"));

        var result = await _service.GetPorId(1);

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

 

    [Fact]
    public async Task Update_DeveRetornar204_QuandoDadosValidos()
    {
        _repository.GetPorId(1).Returns(CriarProduto());
        _repository.Update(Arg.Any<Produto>()).Returns(Task.CompletedTask);

        var result = await _service.Update(1, CriarDto("Notebook Atualizado", 5999m, 8));

        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_DeveRetornar404_QuandoProdutoNaoExiste()
    {
        _repository.GetPorId(999).Returns((Produto?)null);

        var result = await _service.Update(999, CriarDto());

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await _repository.DidNotReceive().Update(Arg.Any<Produto>());
    }

    [Fact]
    public async Task Update_DeveRetornar400_QuandoPrecoInvalido()
    {
        _repository.GetPorId(1).Returns(CriarProduto());

        var result = await _service.Update(1, CriarDto(preco: -10m));

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await _repository.DidNotReceive().Update(Arg.Any<Produto>());
    }

    [Fact]
    public async Task Update_DeveRetornar400_QuandoNomeVazio()
    {
        _repository.GetPorId(1).Returns(CriarProduto());

        var result = await _service.Update(1, CriarDto(nome: ""));

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_DeveRetornar500_QuandoRepositoryLancaException()
    {
        _repository.GetPorId(1).Returns(CriarProduto());
        _repository.Update(Arg.Any<Produto>()).Throws(new Exception("DB error"));

        var result = await _service.Update(1, CriarDto());

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
 

    [Fact]
    public async Task Delete_DeveRetornar204_QuandoProdutoExiste()
    {
        _repository.GetPorId(1).Returns(CriarProduto());
        _repository.Update(Arg.Any<Produto>()).Returns(Task.CompletedTask);

        var result = await _service.Delete(1);

        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_DeveDesativarProduto_AoInvesDeExcluirFisicamente()
    {
        var produto = CriarProduto();
        _repository.GetPorId(1).Returns(produto);

        await _service.Delete(1);

        produto.Ativo.Should().BeFalse();
        await _repository.Received(1).Update(Arg.Is<Produto>(p => !p.Ativo));
    }

    [Fact]
    public async Task Delete_DeveRetornar404_QuandoProdutoNaoExiste()
    {
        _repository.GetPorId(999).Returns((Produto?)null);

        var result = await _service.Delete(999);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await _repository.DidNotReceive().Update(Arg.Any<Produto>());
    }

    [Fact]
    public async Task Delete_DeveRetornar500_QuandoRepositoryLancaException()
    {
        _repository.GetPorId(1).Returns(CriarProduto());
        _repository.Update(Arg.Any<Produto>()).Throws(new Exception("DB error"));

        var result = await _service.Delete(1);

        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}