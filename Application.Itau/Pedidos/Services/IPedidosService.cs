using Application.Itau.Pedidos.Dtos.Requests;
using Application.Itau.Pedidos.Dtos.Responses;
using Shared.Itau.Result;

namespace Application.Itau.Pedidos.Services
{
    public interface IPedidosService
    {
        Task<ICommandResult<int?>> Create(PedidoRequestDto dto);
        Task<ICommandResult> Delete(int id);
        Task<ICommandResult<List<PedidoCompletoDto>>> GetAll();
        Task<ICommandResult<PedidoCompletoDto?>> GetPorId(int id);
        Task<ICommandResult> UpdateStatus(int id, PedidoStatusRequestDto dto);
    }
}
