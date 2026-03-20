using Application.Itau.Pedidos.Dtos.Responses;
using Domain.Itau.Agregados.PedidoAgregado;

namespace Application.Itau.Pedidos.Presenters;

public static class PedidoCompletoPresenterExtensions
{
    public static PedidoCompletoDto ToDto(this Pedido pedido)
    {
        return new PedidoCompletoDto
        {
            Id = pedido.Id,
            NumeroPedido = pedido.NumeroPedido,
            ClienteNome = pedido.ClienteNome,
            ClienteEmail = pedido.ClienteEmail,
            DataPedido = pedido.DataPedido,
            Status = pedido.Status,
            ValorTotal = pedido.ValorTotal,
            Itens = pedido.Itens
                .Select(x => x.ToDto())
                .ToList()
        };
    }

    public static List<PedidoCompletoDto> ToDtoList(this IEnumerable<Pedido> pedidos)
    {
        return pedidos
            .Select(p => p.ToDto())
            .ToList();
    }
}


