using Domain.Itau.Entities.Produto;
using FluentAssertions;

namespace Tests.Itau.Domain;
public class ProdutoTests
{

    [Fact]
    public void Construtor_DeveCriarProduto_QuandoDadosValidos()
    {
        var produto = new Produto("Notebook", 4999.99m, "Notebook i7", 10);

        produto.Nome.Should().Be("Notebook");
        produto.Preco.Should().Be(4999.99m);
        produto.Descricao.Should().Be("Notebook i7");
        produto.Estoque.Should().Be(10);
        produto.Ativo.Should().BeTrue();
        produto.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Construtor_DeveUsarValoresPadrao_QuandoOpcionaisOmitidos()
    {
        var produto = new Produto("Mouse", 49.90m);

        produto.Descricao.Should().BeNull();
        produto.Estoque.Should().Be(0);
        produto.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Construtor_DeveFazerTrimNoNome()
    {
        var produto = new Produto("  Teclado  ", 299.90m);
        produto.Nome.Should().Be("Teclado");
    }
 

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_DeveLancarArgumentException_QuandoNomeVazioOuNulo(string nome)
    {
        var act = () => new Produto(nome, 99.90m);
        act.Should().Throw<ArgumentException>().WithParameterName("nome");
    }

    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoNomeAcimaDeMaxLength()
    {
        var nomeInvalido = new string('A', 101);
        var act = () => new Produto(nomeInvalido, 99.90m);
        act.Should().Throw<ArgumentException>().WithParameterName("nome");
    }

    [Fact]
    public void Construtor_DeveAceitar_QuandoNomeExatamente100Caracteres()
    {
        var nome = new string('A', 100);
        var act = () => new Produto(nome, 99.90m);
        act.Should().NotThrow();
    }
 
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void Construtor_DeveLancarArgumentException_QuandoPrecoMenorOuIgualAZero(decimal preco)
    {
        var act = () => new Produto("Produto", preco);
        act.Should().Throw<ArgumentException>().WithParameterName("preco");
    }

    [Fact]
    public void Construtor_DeveAceitar_QuandoPrecoMinimo()
    {
        var act = () => new Produto("Produto", 0.01m);
        act.Should().NotThrow();
    }


    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoDescricaoAcimaDeMaxLength()
    {
        var descricaoInvalida = new string('X', 501);
        var act = () => new Produto("Produto", 99.90m, descricaoInvalida);
        act.Should().Throw<ArgumentException>().WithParameterName("descricao");
    }

    [Fact]
    public void Construtor_DeveAceitar_QuandoDescricaoExatamente500Caracteres()
    {
        var descricao = new string('X', 500);
        var act = () => new Produto("Produto", 99.90m, descricao);
        act.Should().NotThrow();
    }

    [Fact]
    public void Construtor_DeveAceitar_QuandoDescricaoNula()
    {
        var act = () => new Produto("Produto", 99.90m, null);
        act.Should().NotThrow();
    }
 
    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoEstoqueNegativo()
    {
        var act = () => new Produto("Produto", 99.90m, estoque: -1);
        act.Should().Throw<ArgumentException>().WithParameterName("estoque");
    }

    [Fact]
    public void Construtor_DeveAceitar_QuandoEstoqueZero()
    {
        var act = () => new Produto("Produto", 99.90m, estoque: 0);
        act.Should().NotThrow();
    }
 

    [Fact]
    public void AtualizarNome_DeveAlterarNome_QuandoValido()
    {
        var produto = new Produto("Notebook", 4999.99m);
        produto.AtualizarNome("Notebook Gamer");
        produto.Nome.Should().Be("Notebook Gamer");
    }

    [Fact]
    public void AtualizarNome_DeveLancarArgumentException_QuandoNomeVazio()
    {
        var produto = new Produto("Notebook", 4999.99m);
        var act = () => produto.AtualizarNome("");
        act.Should().Throw<ArgumentException>().WithParameterName("nome");
    }

    [Fact]
    public void AtualizarPreco_DeveAlterarPreco_QuandoValido()
    {
        var produto = new Produto("Notebook", 4999.99m);
        produto.AtualizarPreco(5499.99m);
        produto.Preco.Should().Be(5499.99m);
    }

    [Fact]
    public void AtualizarPreco_DeveLancarArgumentException_QuandoPrecoInvalido()
    {
        var produto = new Produto("Notebook", 4999.99m);
        var act = () => produto.AtualizarPreco(0);
        act.Should().Throw<ArgumentException>().WithParameterName("preco");
    }

    [Fact]
    public void AtualizarDescricao_DeveAlterarDescricao_QuandoValida()
    {
        var produto = new Produto("Notebook", 4999.99m, "Desc antiga");
        produto.AtualizarDescricao("Desc nova");
        produto.Descricao.Should().Be("Desc nova");
    }

    [Fact]
    public void AtualizarEstoque_DeveAlterarEstoque_QuandoValido()
    {
        var produto = new Produto("Notebook", 4999.99m, estoque: 10);
        produto.AtualizarEstoque(50);
        produto.Estoque.Should().Be(50);
    }

    [Fact]
    public void AtualizarEstoque_DeveLancarArgumentException_QuandoEstoqueNegativo()
    {
        var produto = new Produto("Notebook", 4999.99m);
        var act = () => produto.AtualizarEstoque(-1);
        act.Should().Throw<ArgumentException>().WithParameterName("estoque");
    }
 
    [Fact]
    public void Desativar_DeveDefinirAtivoComoFalse()
    {
        var produto = new Produto("Notebook", 4999.99m);
        produto.Desativar();
        produto.Ativo.Should().BeFalse();
    }

    [Fact]
    public void Ativar_DeveDefinirAtivoComoTrue()
    {
        var produto = new Produto("Notebook", 4999.99m);
        produto.Desativar();
        produto.Ativar();
        produto.Ativo.Should().BeTrue();
    }
}