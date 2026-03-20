using Application.Itau.Produtos.Dtos.Requests;
using Application.Itau.Produtos.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Itau.Result;

namespace Api.Itau.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutosService _produtosService;

        public ProdutosController(IProdutosService produtosService)
        {
            _produtosService = produtosService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAtivos()
        {
            var result = await _produtosService.GetAllAtivos();
            return result.ToResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId([FromRoute] int id) 
        {
            var result = await _produtosService.GetPorId(id);
            return result.ToResult();
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProdutoRequestDto dto)
        {
            var result = await _produtosService.Create(dto);
            return result.ToResult();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ProdutoRequestDto dto)
        {
            var result = await _produtosService.Update(id,dto);
            return result.ToResult();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _produtosService.Delete(id);
            return result.ToResult();
        }
    }
}
