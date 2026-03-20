using Application.Itau.Pedidos.Dtos.Requests;
using Application.Itau.Pedidos.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Itau.Result;

namespace Api.Itau.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidosService _pedidosService;

        public PedidosController(IPedidosService pedidosService)
        {
            _pedidosService = pedidosService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _pedidosService.GetAll();
            return result.ToResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId([FromRoute] int id)
        {
            var result = await _pedidosService.GetPorId(id);
            return result.ToResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PedidoRequestDto dto)
        {
            var result = await _pedidosService.Create(dto);
            return result.ToResult();
        }


        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] PedidoStatusRequestDto dto)
        {
            var result = await _pedidosService.UpdateStatus(id, dto);
            return result.ToResult();

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _pedidosService.Delete(id);
            return result.ToResult();
        }
    }
}
