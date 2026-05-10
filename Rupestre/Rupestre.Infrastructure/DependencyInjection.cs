using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;
using Rupestre.Infrastructure.Identity;
using Rupestre.Infrastructure.Repositories;


namespace Rupestre.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddHttpContextAccessor();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit           = false;
            options.Password.RequiredLength         = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase       = false;
            options.Password.RequireLowercase       = false;
            options.User.AllowedUserNameCharacters  = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

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
        services.AddScoped<ISuprimentoRepository, SuprimentoRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();

        return services;
    }
}
