namespace Domain.Itau.Entities.Produto.Repositories
{
    public interface IProdutosRepository
    {
        Task<Produto> Create(Produto produto);
        Task<List<Produto>> GetAllAtivos();
        Task<Produto?> GetPorId(int id);
        Task<List<Produto>> GetPorIds(HashSet<int> idsProdutos);
        Task Update(Produto produto);
    }
}
