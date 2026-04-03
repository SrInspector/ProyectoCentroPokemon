using CentroPokemon.BC.Entidades;
using CentroPokemon.BC.Enumeradores;
using CentroPokemon.BC.Seguridad;
using CentroPokemon.DA.Contexto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace CentroPokemon.DA.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(CentroPokemonDbContext context, IConfiguration configuration)
    {
        await EnsureSchemaReadyAsync(context);

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

    private static async Task EnsureSchemaReadyAsync(CentroPokemonDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await TableExistsAsync(context, "Usuarios"))
        {
            return;
        }

        var databaseCreator = context.GetService<IRelationalDatabaseCreator>();
        await databaseCreator.CreateTablesAsync();
    }

    private static async Task<bool> TableExistsAsync(CentroPokemonDbContext context, string tableName)
    {
        var connection = context.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != System.Data.ConnectionState.Open;

        if (shouldCloseConnection)
        {
            await connection.OpenAsync();
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT COUNT(*)
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @tableName
                """;

            var parameter = command.CreateParameter();
            parameter.ParameterName = "@tableName";
            parameter.Value = tableName;
            command.Parameters.Add(parameter);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await connection.CloseAsync();
            }
        }
    }
}
