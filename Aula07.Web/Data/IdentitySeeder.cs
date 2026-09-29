using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Aula07.Web.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // 1. Desafio: Roles Admin, Gerente e Usuario
        string[] roles = ["Admin", "Gerente", "Usuario"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Usuários padrão para testes imediatos
        var seedUsers = new[]
        {
            new
            {
                Email = "admin@aula07.com",
                Password = "Admin123!",
                Role = "Admin",
                Departamento = "TI"
            },
            new
            {
                Email = "gerente@aula07.com",
                Password = "Gerente123!",
                Role = "Gerente",
                Departamento = "Operações"
            },
            new
            {
                Email = "usuario@aula07.com",
                Password = "Usuario123!",
                Role = "Usuario",
                Departamento = "Vendas"
            }
        };

        foreach (var seedUser in seedUsers)
        {
            var existingUser = await userManager.FindByEmailAsync(seedUser.Email);
            if (existingUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = seedUser.Email,
                    Email = seedUser.Email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, seedUser.Password);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, seedUser.Role);
                    // 3. Desafio: Claim Departamento
                    await userManager.AddClaimAsync(user, new Claim("Departamento", seedUser.Departamento));
                }
            }
        }
    }
}
