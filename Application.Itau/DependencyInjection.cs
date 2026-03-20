using Application.Itau.Pedidos.Services;
using Application.Itau.Produtos.Services;
using Domain.Itau.Agregados.PedidoAgregado.Repositories;
using Domain.Itau.Entities.Produto.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Itau
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddScoped<IPedidosService, PedidosService>();
            services.AddScoped<IProdutosService, ProdutosService>();
             
            return services;
        }
    }
}
