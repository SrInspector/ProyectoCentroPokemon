using CentroPokemon.BC.Entidades;
using CentroPokemon.BC.Enumeradores;
using CentroPokemon.BC.Seguridad;
using CentroPokemon.DA.Contexto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CentroPokemon.DA.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(CentroPokemonDbContext context, IConfiguration configuration)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Usuarios.AnyAsync())
        {
            return;
        }

        var adminCorreo = configuration["SeedAdmin:Correo"] ?? "admin@centropokemon.local";
        var adminPassword = configuration["SeedAdmin:Password"] ?? "Admin1234!";
        var adminNombre = configuration["SeedAdmin:NombreCompleto"] ?? "Administrador Centro Pokemon";

        context.Usuarios.Add(new Usuario
        {
            NombreCompleto = adminNombre,
            Correo = adminCorreo.ToLowerInvariant(),
            PasswordHash = PasswordHasherBC.Hash(adminPassword),
            Rol = RolSistema.Administrador
        });

        await context.SaveChangesAsync();
    }
}
