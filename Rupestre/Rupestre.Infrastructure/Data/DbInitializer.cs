using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rupestre.Infrastructure.Identity;

namespace Rupestre.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var config      = services.GetRequiredService<IConfiguration>();

        // Garante que os perfis existam
        foreach (var role in new[] { "Gerente", "Vendedor" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var userName = config["Auth:Usuario"] ?? "admin";
        var password = config["Auth:Senha"]   ?? "Rupestre@2024";

        if (!userManager.Users.Any())
        {
            // Primeiro acesso: cria o admin como Gerente
            var user = new ApplicationUser
            {
                UserName       = userName,
                Email          = $"{userName}@rupestre.local",
                NomeCompleto   = "Administrador",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var erros = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Falha ao criar usuário admin: {erros}");
            }

            await userManager.AddToRoleAsync(user, "Gerente");
        }
        else
        {
            // Migrações já existentes: garante que o admin tenha o perfil Gerente
            var adminUser = await userManager.FindByNameAsync(userName);
            if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Gerente"))
                await userManager.AddToRoleAsync(adminUser, "Gerente");
        }
    }
}
