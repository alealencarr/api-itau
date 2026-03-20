using Application.Itau.Produtos.Dtos.Requests;
using Application.Itau.Produtos.Dtos.Responses;
using Domain.Itau.Entities.Produto;
using Domain.Itau.Entities.Produto.Repositories;
using Microsoft.Extensions.Logging;
using Shared.Itau;
using Shared.Itau.Result;
using System.Net;

namespace Application.Itau.Produtos.Services
{
    public class ProdutosService : IProdutosService
    {
        private readonly IProdutosRepository _produtoRepository;

        private readonly ILogger<ProdutosService> _logger;

        public ProdutosService(IProdutosRepository produtoRepository, ILogger<ProdutosService> logger)
        {
            _produtoRepository = produtoRepository;
            _logger = logger;
        }

        public async Task<ICommandResult<int?>> Create(ProdutoRequestDto dto)
        {
            try
            {
                _logger.LogInformation("ProdutosService-Create: Recebida requisição para criação de produto: {Nome}", dto.Nome);

                var produto = new Produto(dto.Nome, dto.Preco, dto.Descricao, dto.Estoque);

                await _produtoRepository.Create(produto);

                _logger.LogInformation("ProdutosService-Create: Produto criado com sucesso. Id: {Id}", produto.Id);

                return new CommandResult<int?> { Data = produto.Id, Message = $"Produto criado com sucesso! Id do produto: {produto.Id}", StatusCode = HttpStatusCode.Created };
            }
          catch (Exception ex) when (
                ex is ArgumentException ||
                ex is InvalidOperationException)
            {
                _logger.LogError(ex, "ProdutosService-Create: Erro de validação ao criar produto.");

                return new CommandResult<int?> { Message = $"Erro ao criar produto. Error: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProdutosService-Create: Erro interno no servidor.");

                return new CommandResult<int?> { Message = "Erro interno no servidor", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<ICommandResult<List<ProdutoDto>>> GetAllAtivos()
        {
            try
            {
                _logger.LogInformation("ProdutosService-GetAllAtivos: Recebida requisição para buscar todos os produtos ativos na base de dados.");

                var result = await _produtoRepository.GetAllAtivos();

                var resultToDto = result.Select(x => new ProdutoDto
                {
                    Ativo = x.Ativo,
                    DataCriacao = x.DataCriacao,
                    Descricao = x.Descricao,
                    Estoque = x.Estoque,
                    Id = x.Id,
                    Nome = x.Nome,
                    Preco = x.Preco
                }).ToList();

                _logger.LogInformation("ProdutosService-GetAllAtivos: Produtos localizados na base de dados e retornados com sucesso.");

                return new CommandResult<List<ProdutoDto>> { Data = resultToDto, Message = "Produtos localizados!", StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProdutosService-GetAllAtivos: Erro interno no servidor.");

                return new CommandResult<List<ProdutoDto>> { Message = "Erro interno no servidor", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<ICommandResult<ProdutoDto?>> GetPorId(int id)
        {
            try
            {
                _logger.LogInformation("ProdutosService-GetPorId: Recebida requisição para buscar o produto ID {Id} na base de dados.", id);

                var result = await _produtoRepository.GetPorId(id);

                if (result is null)
                {
                    _logger.LogWarning("ProdutosService-GetPorId: O produto ID {Id} não foi localizado na base de dados.", id);

                    return new CommandResult<ProdutoDto?> { Message = $"Não foi localizado nenhum produto com este id: {id}", StatusCode = HttpStatusCode.NotFound };
                }

                var resultToDto = new ProdutoDto
                {
                    Ativo = result.Ativo,
                    DataCriacao = result.DataCriacao,
                    Descricao = result.Descricao,
                    Estoque = result.Estoque,
                    Id = result.Id,
                    Nome = result.Nome,
                    Preco = result.Preco
                };

                _logger.LogInformation("ProdutosService-GetPorId: O produto ID {Id} foi localizado na base de dados e retornado com sucesso.", id);

                return new CommandResult<ProdutoDto?> { Data = resultToDto, Message = "Produto localizado!", StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProdutosService-GetPorId: Erro interno no servidor.");

                return new CommandResult<ProdutoDto?> { Message = "Erro interno no servidor", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<ICommandResult> Update(int id, ProdutoRequestDto dto)
        {
            try
            {
                _logger.LogInformation("ProdutosService-Update: Recebida requisição para alterar o produto ID {Id} na base de dados.", id);

                var result = await _produtoRepository.GetPorId(id);

                if (result is null)
                {
                    _logger.LogWarning("ProdutosService-Update: O produto ID {Id} não foi localizado na base de dados.", id);

                    return new CommandResult { Message = $"Não foi localizado nenhum produto com este id: {id}", StatusCode = HttpStatusCode.NotFound };
                }

                result.AtualizarNome(dto.Nome);
                result.AtualizarPreco(dto.Preco);
                result.AtualizarDescricao(dto.Descricao);
                result.AtualizarEstoque(dto.Estoque);

                await _produtoRepository.Update(result);

                _logger.LogInformation("ProdutosService-Update: O produto ID {Id} foi alterado na base de dados com sucesso.", id);

                return new CommandResult { StatusCode = HttpStatusCode.NoContent };
            }
            catch (Exception ex) when (
                 ex is ArgumentException ||
                 ex is InvalidOperationException)
            {
                _logger.LogError(ex, "ProdutosService-Update: Erro de validação ao alterar produto ID: {Id}.", id);

                return new CommandResult { Message = $"Erro ao atualizar produto. Error: {ex.Message}", StatusCode = HttpStatusCode.BadRequest };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProdutosService-Update: Erro interno no servidor.");

                return new CommandResult { Message = "Erro interno no servidor", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<ICommandResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("ProdutosService-Delete: Recebida requisição para excluir o produto ID {Id} na base de dados.", id);

                var result = await _produtoRepository.GetPorId(id);

                if (result is null)
                {
                    _logger.LogWarning("ProdutosService-Delete: O produto ID {Id} não foi localizado na base de dados.", id);

                    return new CommandResult { Message = $"Não foi localizado nenhum produto com este id: {id}", StatusCode = HttpStatusCode.NotFound };
                }

                result.Desativar();

                await _produtoRepository.Update(result);

                _logger.LogInformation("ProdutosService-Delete: O produto ID {Id} foi excluido da base de dados com sucesso.", id);

                return new CommandResult { StatusCode = HttpStatusCode.NoContent };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProdutosService-Delete: Erro interno no servidor.");

                return new CommandResult { Message = "Erro interno no servidor", StatusCode = HttpStatusCode.InternalServerError };

            }
        }

    }
}
