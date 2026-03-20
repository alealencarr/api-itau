using System.ComponentModel.DataAnnotations;

namespace Domain.Itau.Entities.Produto;

public class Produto
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public string? Descricao { get; private set; }
    public decimal Preco { get; private set; }
    public int Estoque { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public bool Ativo { get; private set; }

    protected Produto() { } // EF Core

    public Produto(string nome, decimal preco, string? descricao = null, int estoque = 0)
    {
        ValidarNome(nome);
        ValidarPreco(preco);
        ValidarDescricao(descricao);
        ValidarEstoque(estoque);

        Nome = nome.Trim();
        Preco = preco;
        Descricao = descricao?.Trim();
        Estoque = estoque;
        DataCriacao = DateTime.UtcNow;
        Ativo = true;
    }

    public void AtualizarNome(string nome)
    {
        ValidarNome(nome);
        Nome = nome.Trim();
    }

    public void AtualizarPreco(decimal preco)
    {
        ValidarPreco(preco);
        Preco = preco;
    }

    public void AtualizarDescricao(string? descricao)
    {
        ValidarDescricao(descricao);
        Descricao = descricao?.Trim();
    }

    public void AtualizarEstoque(int estoque)
    {
        ValidarEstoque(estoque);
        Estoque = estoque;
    }

    public void Ativar() => Ativo = true;
    public void Desativar() => Ativo = false;

    // --- Validações privadas ---

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do produto é obrigatório.", nameof(nome));

        if (nome.Trim().Length > 100)
            throw new ArgumentException("O nome do produto não pode ultrapassar 100 caracteres.", nameof(nome));
    }

    private static void ValidarPreco(decimal preco)
    {
        if (preco <= 0)
            throw new ArgumentException("O preço do produto deve ser maior que zero.", nameof(preco));
    }

    private static void ValidarDescricao(string? descricao)
    {
        if (descricao is not null && descricao.Trim().Length > 500)
            throw new ArgumentException("A descrição não pode ultrapassar 500 caracteres.", nameof(descricao));
    }

    private static void ValidarEstoque(int estoque)
    {
        if (estoque < 0)
            throw new ArgumentException("O estoque não pode ser negativo.", nameof(estoque));
    }
}