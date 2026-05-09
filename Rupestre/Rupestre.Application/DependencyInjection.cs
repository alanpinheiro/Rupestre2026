using Microsoft.Extensions.DependencyInjection;
using Rupestre.Application.Interfaces;
using Rupestre.Application.Services;

namespace Rupestre.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICaixaService, CaixaService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IDespesaService, DespesaService>();
        services.AddScoped<IFabricanteService, FabricanteService>();
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<IVendedorService, VendedorService>();
        services.AddScoped<IVendaService, VendaService>();
        services.AddScoped<IFormaPagamentoService, FormaPagamentoService>();
        services.AddScoped<IVendaRemessaService, VendaRemessaService>();
        services.AddScoped<ISuprimentoService, SuprimentoService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
