using Microsoft.Extensions.DependencyInjection;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;
using Rupestre.Infrastructure.Repositories;

namespace Rupestre.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton(new ConnectionManager(connectionString));

        services.AddScoped<ICaixaRepository, CaixaRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IDespesaRepository, DespesaRepository>();
        services.AddScoped<IFabricanteRepository, FabricanteRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IVendedorRepository, VendedorRepository>();
        services.AddScoped<IVendaRepository, VendaRepository>();
        services.AddScoped<IVendaProdutoRepository, VendaProdutoRepository>();
        services.AddScoped<IVendaPagamentoRepository, VendaPagamentoRepository>();
        services.AddScoped<IFormaPagamentoRepository, FormaPagamentoRepository>();
        services.AddScoped<IVendaRemessaRepository, VendaRemessaRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();

        return services;
    }
}
