using Application.Itau.Pedidos.Dtos.Requests;
using Application.Itau.Pedidos.Dtos.Responses;
using Application.Itau.Pedidos.Models;
using Application.Itau.Pedidos.Presenters;
using Domain.Itau.Agregados.PedidoAgregado;
using Domain.Itau.Agregados.PedidoAgregado.Repositories;
using Domain.Itau.Entities.Produto.Repositories;
using Microsoft.Extensions.Logging;
using Shared.Itau;
using Shared.Itau.Result;
using System.Net;

namespace Application.Itau.Pedidos.Services
{
    public class PedidosService : IPedidosService
    {
        private readonly IPedidosRepository _pedidosRepository;
        private readonly IProdutosRepository _produtosRepository;
        private readonly ILogger<PedidosService> _logger;
        public PedidosService(IPedidosRepository pedidosRepository, ILogger<PedidosService> logger, IProdutosRepository produtosRepository)
        {
            _pedidosRepository = pedidosRepository;
            _logger = logger;
            _produtosRepository = produtosRepository;
        }

        public async Task<ICommandResult<int?>> Create(PedidoRequestDto dto)
        {
            try
            {
                _logger.LogInformation("PedidosService-Create: Recebida requisição para criação do pedido: {Numero}", dto.NumeroPedido);

                var pedidoDuplicado = await _pedidosRepository.GetPorNumero(dto.NumeroPedido);

                if (pedidoDuplicado is not null)
                {
                    return new CommandResult<int?> { Message = "Já existe um produto com este número, favor informar outro número.", StatusCode = HttpStatusCode.BadRequest };
                }

                var dataIsValid = await IdsProductsIsValid(dto); //Valida se existem no DB e se são todos ativos.

                if (!dataIsValid.Data.isValid)
                {
                    return new CommandResult<int?> { Message = dataIsValid.Message, StatusCode = dataIsValid.StatusCode };
                }

                var pedidosDb = dataIsValid.Data.pedidos;

                var itensPedido = pedidosDb!.Select(x => new ItemPedido(x.Produto, x.Quantidade)).ToList();

                var pedido = new Pedido(dto.NumeroPedido, dto.ClienteNome, dto.ClienteEmail, itensPedido);

                await _pedidosRepository.Create(pedido);

                _logger.LogInformation("PedidosService-Create: Pedido criado com sucesso. Id: {Id}", pedido.Id);

                return new CommandResult<int?> { Data = pedido.Id, Message = $"Pedido criado com sucesso! Id do pedido: {pedido.Id}", StatusCode = HttpStatusCode.Created };
            }
            catch (Exception ex) when (
                ex is ArgumentException ||
                ex is InvalidOperationException)
            {
                _logger.LogError(ex, "PedidosService-Create: Erro de validação ao criar pedido.");

                return new CommandResult<int?> { Message = $"Erro ao criar pedido. Error: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PedidosService-Create: Erro interno no servidor.");

                return new CommandResult<int?> { Message = "Erro interno no servidor", StatusCode = HttpStatusCode.InternalServerError };
            }
        }
        public async Task<ICommandResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("PedidosService-Delete: Recebida requisição para excluir o pedido ID {Id} na base de dados.", id);

                var result = await _pedidosRepository.GetPorId(id);

                if (result is null)
                {
                    _logger.LogWarning("PedidosService-Delete: O pedido ID {Id} não foi localizado na base de dados.", id);

                    return new CommandResult { Message = $"Não foi localizado nenhum pedido com este id: {id}", StatusCode = HttpStatusCode.NotFound };
                }

                result.CancelarPedido();

                await _pedidosRepository.Update(result);

                _logger.LogInformation("PedidosService-Delete: O pedido ID {Id} foi excluido da base de dados com sucesso.", id);

                return new CommandResult { StatusCode = HttpStatusCode.NoContent };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PedidosService-Delete: Erro interno no servidor.");

                return new CommandResult { Message = "Erro interno no servidor", StatusCode = HttpStatusCode.InternalServerError };

            }
        }
        public async Task<ICommandResult<List<PedidoCompletoDto>>> GetAll()
        {
            try
            {
                _logger.LogInformation("PedidosService-GetAll: Recebida requisição para buscar todos os pedidos na base de dados.");

                var result = await _pedidosRepository.GetAll();

                var resultToDto = result.ToDtoList();

                _logger.LogInformation("PedidosService-GetAll: Pedidos localizados na base de dados e retornados com sucesso.");

                return new CommandResult<List<PedidoCompletoDto>> { Data = resultToDto, Message = "Pedidos localizados!", StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PedidosService-GetAll: Erro interno no servidor.");

                return new CommandResult<List<PedidoCompletoDto>> { Message = "Erro interno no servidor", StatusCode = HttpStatusCode.InternalServerError };
            }
        }
        public async Task<ICommandResult<PedidoCompletoDto?>> GetPorId(int id)
        {
            try
            {
                _logger.LogInformation("PedidosService-GetPorId: Recebida requisição para buscar o pedido ID {Id} na base de dados.", id);

                var result = await _pedidosRepository.GetPorId(id);

                if (result is null)
                {
                    _logger.LogWarning("PedidosService-GetPorId: O pedido ID {Id} não foi localizado na base de dados.", id);

                    return new CommandResult<PedidoCompletoDto?> { Message = $"Não foi localizado nenhum pedido com este id: {id}", StatusCode = HttpStatusCode.NotFound };
                }

                var resultToDto = result.ToDto();

                _logger.LogInformation("PedidosService-GetPorId: O pedido ID {Id} foi localizado na base de dados e retornado com sucesso.", id);

                return new CommandResult<PedidoCompletoDto?> { Data = resultToDto, Message = "Pedido localizado!", StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PedidosService-GetPorId: Erro interno no servidor.");

                return new CommandResult<PedidoCompletoDto?> { Message = "Erro interno no servidor", StatusCode = HttpStatusCode.InternalServerError };
            }
        }
        public async Task<ICommandResult> UpdateStatus(int id, PedidoStatusRequestDto dto)
        {
            try
            {
                _logger.LogInformation("PedidosService-UpdateStatus: Recebida requisição para alterar o status do pedido ID {Id} na base de dados.", id);

                var result = await _pedidosRepository.GetPorId(id);

                if (result is null)
                {
                    _logger.LogWarning("PedidosService-UpdateStatus: O pedido ID {Id} não foi localizado na base de dados.", id);

                    return new CommandResult { Message = $"Não foi localizado nenhum pedido com este id: {id}", StatusCode = HttpStatusCode.NotFound };
                }

                result.AvancarStatus(dto.Status);

                await _pedidosRepository.Update(result);

                _logger.LogInformation("PedidosService-UpdateStatus: O status do pedido ID {Id} foi alterado na base de dados para {Status} com sucesso.", id, dto.Status);

                return new CommandResult { StatusCode = HttpStatusCode.NoContent };
            }
            catch (Exception ex) when (
                ex is ArgumentException ||
                ex is InvalidOperationException)
            {
                _logger.LogError(ex, "PedidosService-UpdateStatus: Erro de validação ao alterar status do pedido ID: {Id}.", id);

                return new CommandResult { Message = $"Erro ao atualizar status do pedido. Error: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PedidosService-UpdateStatus: Erro interno no servidor.");

                return new CommandResult { Message = "Erro interno no servidor", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        private async Task<ICommandResult<(bool isValid, List<ItemPedidoConsolidadoModel>? pedidos)>> IdsProductsIsValid(PedidoRequestDto dto)
        {
            var itensAgrupados = dto.Itens
                    .GroupBy(x => x.ProdutoId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(x => x.Quantidade)
                    );

            var idsProdutos = itensAgrupados.Keys.ToHashSet();

            var produtos = await _produtosRepository.GetPorIds(idsProdutos);

            var idsEncontrados = produtos
                .Select(p => p.Id)  
                .ToHashSet();

            var idsNaoEncontrados = idsProdutos
                .Except(idsEncontrados)
                .ToList();

            if (idsNaoEncontrados.Any())
            {
                _logger.LogWarning("PedidosService-IsValid: Produtos não encontrados: {Ids}", idsNaoEncontrados);
                return new CommandResult<(bool, List<ItemPedidoConsolidadoModel>?)> { Data = (false, null), Message = $"Os produtos Id's: {string.Join(", ", idsNaoEncontrados)} não foram encontrados.", StatusCode = HttpStatusCode.NotFound };
            }

            var idsInativos = produtos
                             .Where(x => !x.Ativo)
                             .Select(x => x.Id)
                             .ToHashSet();

            if (idsInativos.Any())
            {
                _logger.LogWarning("PedidosService-IsValid: Produtos inativos: {Ids}", idsInativos);
                return new CommandResult<(bool, List<ItemPedidoConsolidadoModel>?)> { Data = (false, null), Message = $"Os produtos Id's: {string.Join(", ", idsInativos)} estão inativos.", StatusCode = HttpStatusCode.BadRequest };
            }
            var itensConsolidados = produtos
                .Select(p => new ItemPedidoConsolidadoModel
                {
                    Produto = p,
                    Quantidade = itensAgrupados[p.Id]
                })
                .ToList();

            return new CommandResult<(bool, List<ItemPedidoConsolidadoModel>?)> { Data = (true, itensConsolidados), StatusCode = HttpStatusCode.OK };
        }

    }
}
