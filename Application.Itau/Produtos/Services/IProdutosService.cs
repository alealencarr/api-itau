using Application.Itau.Produtos.Dtos.Requests;
using Application.Itau.Produtos.Dtos.Responses;
using Shared.Itau.Result;

namespace Application.Itau.Produtos.Services
{
    public interface IProdutosService
    {
        Task<ICommandResult<int?>> Create(ProdutoRequestDto dto);
        Task<ICommandResult> Delete(int id);
        Task<ICommandResult<List<ProdutoDto>>> GetAllAtivos();
        Task<ICommandResult<ProdutoDto?>> GetPorId(int id);
        Task<ICommandResult> Update(int id, ProdutoRequestDto dto);
    }
}
