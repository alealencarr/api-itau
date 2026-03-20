using Domain.Itau.Agregados.PedidoAgregado;
using Domain.Itau.Agregados.PedidoAgregado.Repositories;
using Infra.Itau.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infra.Itau.Repositories.Pedidos
{
    public class PedidoRepository : IPedidosRepository
    {
        private readonly AppDbContext _context;

        public PedidoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Pedido> Create(Pedido pedido)
        {
            await _context.Pedidos.AddAsync(pedido);
            await _context.SaveChangesAsync();
            return pedido;
        }

        public async Task<List<Pedido>> GetAll()
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<Pedido?> GetPorId(int id)
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Pedido>> GetPorIds(HashSet<int> idsPedidos)
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .Where(p => idsPedidos.Contains(p.Id))
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<Pedido?> GetPorNumero(string numeroPedido)
        {
            return await _context.Pedidos 
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.NumeroPedido == numeroPedido);
        }

        public async Task Update(Pedido pedido)
        {
            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();
        }
    }
}
