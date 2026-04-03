using System.Text;
using CentroPokemon.BC.Enumeradores;
using CentroPokemon.BW.Interfaces.BW;
using CentroPokemon.BW.Interfaces.DA;

namespace CentroPokemon.BW.CU;

public class GestionarHistorialBW : IGestionarHistorialBW
{
    private readonly IPokemonDA _pokemonDA;
    private readonly ITratamientoDA _tratamientoDA;
    private readonly IInternamientoDA _internamientoDA;
    private readonly ICitaDA _citaDA;
    private readonly IFacturaDA _facturaDA;
    private readonly IUsuarioDA _usuarioDA;
    private readonly IAuditoriaBW _auditoriaBW;

    public GestionarHistorialBW(IPokemonDA pokemonDA, ITratamientoDA tratamientoDA, IInternamientoDA internamientoDA, ICitaDA citaDA, IFacturaDA facturaDA, IUsuarioDA usuarioDA, IAuditoriaBW auditoriaBW)
    {
        _pokemonDA = pokemonDA;
        _tratamientoDA = tratamientoDA;
        _internamientoDA = internamientoDA;
        _citaDA = citaDA;
        _facturaDA = facturaDA;
        _usuarioDA = usuarioDA;
        _auditoriaBW = auditoriaBW;
    }

    public async Task<object?> ObtenerHistorialPokemonAsync(int usuarioId, string rol, int pokemonId, string? tipo, string? estado, DateTime? fechaInicioUtc, DateTime? fechaFinUtc)
    {
        var pokemon = await ValidarAccesoAsync(usuarioId, rol, pokemonId);
        if (pokemon is null)
        {
            return null;
        }

        var tratamientos = await _tratamientoDA.ObtenerPorPokemonAsync(pokemonId);
        var internamientos = await _internamientoDA.ListarAsync();
        var citas = await _citaDA.ListarAsync();

        tratamientos = tratamientos
            .Where(x => (string.IsNullOrWhiteSpace(tipo) || x.Tipo.Contains(tipo, StringComparison.OrdinalIgnoreCase)) &&
                        (!fechaInicioUtc.HasValue || x.FechaInicioUtc >= fechaInicioUtc.Value) &&
                        (!fechaFinUtc.HasValue || x.FechaFinUtc <= fechaFinUtc.Value) &&
                        (string.IsNullOrWhiteSpace(estado) || x.Estado.ToString().Equals(estado, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var historial = ConstruirHistorialPlano(
            pokemon,
            tratamientos,
            internamientos.Where(x => x.PokemonId == pokemonId),
            citas.Where(x => x.PokemonId == pokemonId));

        return historial;
    }

    public async Task<byte[]> ExportarExpedienteAsync(int usuarioId, string rol, int pokemonId, string formato)
    {
        var historial = await ObtenerHistorialPokemonAsync(usuarioId, rol, pokemonId, null, null, null, null)
            ?? throw new InvalidOperationException("No se encontro informacion para el pokemon.");

        var json = System.Text.Json.JsonSerializer.Serialize(historial, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });

        await _auditoriaBW.RegistrarAsync(usuarioId, "Pokemon", pokemonId.ToString(), TipoOperacionAuditoria.Exportar, $"Expediente exportado en formato {formato}.");
        if (formato.Equals("pdf", StringComparison.OrdinalIgnoreCase))
        {
            return SimplePdfGenerator.Generate("Expediente Clinico", json);
        }

        return Encoding.UTF8.GetBytes(json);
    }

    private static object ConstruirHistorialPlano(
        BC.Entidades.Pokemon pokemon,
        IEnumerable<BC.Entidades.TratamientoMedico> tratamientos,
        IEnumerable<BC.Entidades.Internamiento> internamientos,
        IEnumerable<BC.Entidades.Cita> citas)
    {
        return new
        {
            Pokemon = new
            {
                pokemon.Id,
                pokemon.IdentificadorUnico,
                pokemon.Nombre,
                pokemon.Especie,
                pokemon.Nivel,
                pokemon.TipoPrimario,
                pokemon.TipoSecundario,
                EstadoClinico = pokemon.EstadoSalud.ToString(),
                Entrenador = pokemon.Entrenador is null
                    ? null
                    : new
                    {
                        pokemon.Entrenador.Id,
                        pokemon.Entrenador.Identificacion,
                        pokemon.Entrenador.Nombre,
                        pokemon.Entrenador.Email,
                        pokemon.Entrenador.Telefono
                    }
            },
            Tratamientos = tratamientos.Select(x => new
            {
                x.Id,
                x.Tipo,
                x.Dosis,
                x.Frecuencia,
                x.FechaInicioUtc,
                x.FechaFinUtc,
                x.EsCritico,
                Estado = x.Estado.ToString(),
                Responsable = x.UsuarioResponsable is null ? null : x.UsuarioResponsable.NombreCompleto
            }),
            Internamientos = internamientos.Select(x => new
            {
                x.Id,
                x.Codigo,
                x.FechaIngresoUtc,
                x.Motivo,
                x.AreaAsignada,
                Estado = x.Estado.ToString(),
                Responsable = x.UsuarioResponsable is null ? null : x.UsuarioResponsable.NombreCompleto
            }),
            Citas = citas.Select(x => new
            {
                x.Id,
                x.FechaProgramadaUtc,
                x.Motivo,
                Estado = x.Estado.ToString(),
                x.RequiereAprobacionAdministrador,
                x.AprobadaPorAdministrador,
                AsignadoA = x.UsuarioAsignado is null ? null : x.UsuarioAsignado.NombreCompleto
            })
        };
    }

    private async Task<BC.Entidades.Pokemon?> ValidarAccesoAsync(int usuarioId, string rol, int pokemonId)
    {
        var pokemon = await _pokemonDA.ObtenerPorIdAsync(pokemonId);
        if (pokemon is null)
        {
            return null;
        }

        if (rol == RolSistema.Entrenador.ToString())
        {
            var usuario = await _usuarioDA.ObtenerPorIdAsync(usuarioId);
            if (usuario?.EntrenadorId != pokemon.EntrenadorId)
            {
                return null;
            }
        }

        return pokemon;
    }
}
