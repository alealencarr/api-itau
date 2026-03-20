namespace Domain.Itau.Agregados.PedidoAgregado;
public static class Status
{
    public const string Pendente = "Pendente";
    public const string Processando = "Processando";
    public const string Enviado = "Enviado";
    public const string Entregue = "Entregue";
    public const string Cancelado = "Cancelado";

    private static readonly HashSet<string> _validos =
        new() { Pendente, Processando, Enviado, Entregue, Cancelado };

    // Transições permitidas: chave = status atual, valor = próximos status válidos
    private static readonly Dictionary<string, HashSet<string>> _transicoes = new()
    {
        [Pendente] = new() { Processando, Cancelado },
        [Processando] = new() { Enviado, Cancelado },
        [Enviado] = new() { Entregue },
        [Entregue] = new(),
        [Cancelado] = new(),
    };

    public static bool EValido(string status) => _validos.Contains(status);

    public static bool TransicaoPermitida(string atual, string proximo) =>
        _transicoes.TryGetValue(atual, out var permitidos) && permitidos.Contains(proximo);
}
