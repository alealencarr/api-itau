using Application.Itau.Pedidos.Dtos.Responses;
using Domain.Itau.Agregados.PedidoAgregado;

namespace Application.Itau.Pedidos.Presenters;
public static class ItemPedidoPresenterExtensions
{
    public static ItemPedidoCompletoDto ToDto(this ItemPedido item)
    {
        return new ItemPedidoCompletoDto
        {
            Id = item.Id,
            ProdutoId = item.ProdutoId,
            Quantidade = item.Quantidade,
            PrecoUnitario = item.PrecoUnitario,
            Subtotal = item.Subtotal
        };
    }

    public static List<ItemPedidoCompletoDto> ToDtoList(this IEnumerable<ItemPedido> itens)
    {
        return itens
            .Select(i => i.ToDto())
            .ToList();
    }
}