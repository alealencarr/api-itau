using Domain.Itau.Entities.Produto;
using Domain.Itau.Entities.Produto.Repositories;
using Infra.Itau.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infra.Itau.Repositories.Produtos
{
    public class ProdutoRepository : IProdutosRepository
    {
        private readonly AppDbContext _context;

        public ProdutoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Produto> Create(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task<List<Produto>> GetAllAtivos()
        {
            return await _context.Produtos
                .Where(p => p.Ativo)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Produto?> GetPorId(int id)
        {
            return await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Produto>> GetPorIds(HashSet<int> ids)
        {
            return await _context.Produtos
                .Where(p => ids.Contains(p.Id))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task Update(Produto produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }
    }
}
