using Domain.Itau.Agregados.PedidoAgregado.Repositories;
using Domain.Itau.Entities.Produto.Repositories;
using Infra.Itau.DbContexts;
using Infra.Itau.Persistence;
using Infra.Itau.Repositories.Pedidos;
using Infra.Itau.Repositories.Produtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Itau
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("ItauDb"));


            services.AddScoped<IProdutosRepository, ProdutoRepository>();
            services.AddScoped<IPedidosRepository, PedidoRepository>();

            services.AddScoped<DataSeeder>();
            return services;
        }
    }
}
