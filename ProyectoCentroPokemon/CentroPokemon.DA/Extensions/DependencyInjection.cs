using CentroPokemon.BW.CU;
using CentroPokemon.BW.Interfaces.BW;
using CentroPokemon.BW.Interfaces.DA;
using CentroPokemon.DA.Contexto;
using CentroPokemon.DA.Repositorios;
using CentroPokemon.DA.Seguridad;
using CentroPokemon.DA.Servicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CentroPokemon.DA.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCentroPokemonDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = ResolveConnectionString(configuration);

        services.AddDbContext<CentroPokemonDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IUsuarioDA, UsuarioDA>();
        services.AddScoped<IEntrenadorDA, EntrenadorDA>();
        services.AddScoped<IPokemonDA, PokemonDA>();
        services.AddScoped<ICitaDA, CitaDA>();
        services.AddScoped<IAtencionDA, AtencionDA>();
        services.AddScoped<IInventarioDA, InventarioDA>();
        services.AddScoped<IInternamientoDA, InternamientoDA>();
        services.AddScoped<ITratamientoDA, TratamientoDA>();
        services.AddScoped<IServicioClinicoDA, ServicioClinicoDA>();
        services.AddScoped<IFacturaDA, FacturaDA>();
        services.AddScoped<IAuditoriaDA, AuditoriaDA>();
        services.AddScoped<IReporteDA, ReporteDA>();
        services.AddScoped<ITokenDA, TokenDA>();
        services.AddHttpClient<IPokeApiDA, PokeApiDA>(client =>
        {
            client.BaseAddress = new Uri(configuration["PokeApi:BaseUrl"] ?? "https://pokeapi.co/api/v2/");
        });

        services.AddScoped<IAuditoriaBW, AuditoriaBW>();
        services.AddScoped<IAutenticacionBW, AutenticacionBW>();
        services.AddScoped<IGestionarEntrenadoresBW, GestionarEntrenadoresBW>();
        services.AddScoped<IGestionarPokemonBW, GestionarPokemonBW>();
        services.AddScoped<IGestionarCitasBW, GestionarCitasBW>();
        services.AddScoped<IGestionarAtencionesBW, GestionarAtencionesBW>();
        services.AddScoped<IGestionarInventarioBW, GestionarInventarioBW>();
        services.AddScoped<IGestionarInternamientosBW, GestionarInternamientosBW>();
        services.AddScoped<IGestionarTratamientosBW, GestionarTratamientosBW>();
        services.AddScoped<IGestionarFacturacionBW, GestionarFacturacionBW>();
        services.AddScoped<IGestionarHistorialBW, GestionarHistorialBW>();
        services.AddScoped<IGestionarReportesBW, GestionarReportesBW>();

        return services;
    }

    private static string ResolveConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return connectionString;
        }

        var server = configuration["SqlServer:Server"];
        var database = configuration["SqlServer:Database"];

        if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(database))
        {
            throw new InvalidOperationException(
                "Debe configurar ConnectionStrings:DefaultConnection o SqlServer:Server y SqlServer:Database.");
        }

        var trustedConnection = configuration.GetValue("SqlServer:TrustedConnection", true);
        var trustServerCertificate = configuration.GetValue("SqlServer:TrustServerCertificate", true);
        var multipleActiveResultSets = configuration.GetValue("SqlServer:MultipleActiveResultSets", true);

        return $"Server={server};Database={database};Trusted_Connection={trustedConnection};TrustServerCertificate={trustServerCertificate};MultipleActiveResultSets={multipleActiveResultSets}";
    }
}
