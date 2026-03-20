using Domain.Itau.Agregados.PedidoAgregado;
using Domain.Itau.Entities.Produto;
using Infra.Itau.DbContexts;
using Microsoft.Extensions.Logging;

namespace Infra.Itau.Persistence;
public class DataSeeder
{
    private readonly AppDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(AppDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Initialize()
    {
        await SeedProdutos();
        await SeedPedidos();
    }

    private async Task SeedProdutos()
    {
        if (_context.Produtos.Any())
        {
            _logger.LogInformation("DataSeeder-SeedProdutos: Produtos já existem na base de dados. Seed ignorado.");
            return;
        }

        _logger.LogInformation("DataSeeder-SeedProdutos: Inserindo produtos na base de dados...");

        var produtos = new List<Produto>
            {
                new Produto("Notebook Dell Inspiron",    4999.99m,  "Notebook Dell Inspiron i7 16GB RAM 512GB SSD",     estoque: 15),
                new Produto("Mouse Logitech MX Master",  349.90m,  "Mouse sem fio ergonômico com scroll adaptativo",    estoque: 50),
                new Produto("Teclado Mecânico Redragon", 299.90m,  "Teclado mecânico RGB switch blue",                  estoque: 30),
                new Produto("Monitor LG 27\"",          1899.00m,  "Monitor IPS Full HD 75Hz com FreeSync",             estoque: 20),
                new Produto("Headset HyperX Cloud",      599.90m,  "Headset gamer surround 7.1 com microfone removível", estoque: 25),
                new Produto("Webcam Logitech C920",      449.90m,  "Webcam Full HD 1080p com microfone estéreo",         estoque: 40),
                new Produto("SSD Kingston 1TB",          399.90m,  "SSD SATA 2.5\" leitura 550MB/s",                    estoque: 60),
                new Produto("Cadeira Gamer ThunderX3",  1299.00m,  "Cadeira gamer reclinável com apoio lombar",          estoque: 10),
                new Produto("Hub USB-C 7 em 1",          189.90m,  "Hub multiportas HDMI, USB 3.0, SD, PD 100W",        estoque: 35),
                new Produto("Mousepad XL Redragon",       79.90m,  "Mousepad extra large 900x400mm antiderrapante",     estoque: 80),
            };

        await _context.Produtos.AddRangeAsync(produtos);
        await _context.SaveChangesAsync();

        _logger.LogInformation("DataSeeder-SeedProdutos: {Total} produtos inseridos com sucesso.", produtos.Count);
    }

    private async Task SeedPedidos()
    {
        if (_context.Pedidos.Any())
        {
            _logger.LogInformation("DataSeeder-SeedPedidos: Pedidos já existem na base de dados. Seed ignorado.");
            return;
        }

        _logger.LogInformation("DataSeeder-SeedPedidos: Inserindo pedidos na base de dados...");

        var produtos = _context.Produtos.ToList();

        var pedidos = new List<Pedido>
            {
                CriarPedido(
                    "PED-001", "João Silva", "joao.silva@email.com",
                    new List<(Produto, int)>
                    {
                        (produtos[0], 1), // Notebook
                        (produtos[1], 2)  // Mouse x2
                    }
                ),
                CriarPedido(
                    "PED-002", "Maria Oliveira", "maria.oliveira@email.com",
                    new List<(Produto, int)>
                    {
                        (produtos[3], 1), // Monitor
                        (produtos[2], 1), // Teclado
                        (produtos[5], 1)  // Webcam
                    }
                ),
                CriarPedido(
                    "PED-003", "Carlos Pereira", "carlos.pereira@email.com",
                    new List<(Produto, int)>
                    {
                        (produtos[6], 2), // SSD x2
                        (produtos[8], 1)  // Hub USB-C
                    }
                ),
                CriarPedido(
                    "PED-004", "Ana Costa", "ana.costa@email.com",
                    new List<(Produto, int)>
                    {
                        (produtos[7], 1), // Cadeira
                        (produtos[9], 2), // Mousepad x2
                        (produtos[4], 1)  // Headset
                    }
                ),
                CriarPedido(
                    "PED-005", "Bruno Souza", "bruno.souza@email.com",
                    new List<(Produto, int)>
                    {
                        (produtos[1], 1), // Mouse
                        (produtos[2], 1), // Teclado
                        (produtos[9], 1), // Mousepad
                        (produtos[4], 1)  // Headset
                    }
                ),
            };

        pedidos[1].AvancarStatus(Status.Processando);
        pedidos[2].AvancarStatus(Status.Processando);
        pedidos[2].AvancarStatus(Status.Enviado);
        pedidos[3].AvancarStatus(Status.Processando);
        pedidos[3].AvancarStatus(Status.Enviado);
        pedidos[3].AvancarStatus(Status.Entregue);

        await _context.Pedidos.AddRangeAsync(pedidos);
        await _context.SaveChangesAsync();

        _logger.LogInformation("DataSeeder-SeedPedidos: {Total} pedidos inseridos com sucesso.", pedidos.Count);
    }

    private static Pedido CriarPedido(string numero, string clienteNome, string clienteEmail, List<(Produto produto, int quantidade)> itens)
    {
        var itensPedido = itens
            .Select(x => new ItemPedido(x.produto, x.quantidade))
            .ToList();

        return new Pedido(numero, clienteNome, clienteEmail, itensPedido);
    }
}