namespace Domain.Itau.Agregados.PedidoAgregado.Repositories
{
    public interface IPedidosRepository
    {
        Task<Pedido> Create(Pedido pedido);
        Task<List<Pedido>> GetAll();
        Task<Pedido?> GetPorId(int id);
        Task<List<Pedido>> GetPorIds(HashSet<int> idsPedidos);
        Task Update(Pedido pedido);
    }
}
