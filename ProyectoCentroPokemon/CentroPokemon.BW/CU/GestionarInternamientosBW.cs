using CentroPokemon.BC.DTOs.Internamientos;
using CentroPokemon.BC.Entidades;
using CentroPokemon.BC.Enumeradores;
using CentroPokemon.BW.Interfaces.BW;
using CentroPokemon.BW.Interfaces.DA;

namespace CentroPokemon.BW.CU;

public class GestionarInternamientosBW : IGestionarInternamientosBW
{
    private readonly IInternamientoDA _internamientoDA;
    private readonly IPokemonDA _pokemonDA;
    private readonly ITratamientoDA _tratamientoDA;
    private readonly IAuditoriaBW _auditoriaBW;

    public GestionarInternamientosBW(IInternamientoDA internamientoDA, IPokemonDA pokemonDA, ITratamientoDA tratamientoDA, IAuditoriaBW auditoriaBW)
    {
        _internamientoDA = internamientoDA;
        _pokemonDA = pokemonDA;
        _tratamientoDA = tratamientoDA;
        _auditoriaBW = auditoriaBW;
    }

    public async Task<List<Internamiento>> ListarAsync(int usuarioId, string rol, string? area, EstadoInternamiento? estado, DateTime? fechaInicioUtc, DateTime? fechaFinUtc)
    {
        var items = await _internamientoDA.FiltrarAsync(area, estado, fechaInicioUtc, fechaFinUtc);
        if (rol == RolSistema.Entrenador.ToString())
        {
            return items.Where(x => x.Pokemon?.Entrenador?.Usuarios.Any(u => u.Id == usuarioId) == true).ToList();
        }

        return items;
    }

    public async Task<Internamiento?> ObtenerPorIdAsync(int usuarioId, string rol, int id)
    {
        var internamiento = await _internamientoDA.ObtenerPorIdAsync(id);
        if (internamiento is null)
        {
            return null;
        }

        if (rol == RolSistema.Entrenador.ToString() && internamiento.Pokemon?.Entrenador?.Usuarios.Any(u => u.Id == usuarioId) != true)
        {
            return null;
        }

        return internamiento;
    }

    public async Task<Internamiento> CrearAsync(int usuarioId, InternamientoRequest request)
    {
        var pokemon = await _pokemonDA.ObtenerPorIdAsync(request.PokemonId)
            ?? throw new InvalidOperationException("El pokemon indicado no existe.");

        if (await _internamientoDA.ObtenerActivoPorPokemonAsync(request.PokemonId) is not null)
        {
            throw new InvalidOperationException("El pokemon ya tiene un internamiento activo.");
        }

        var internamiento = new Internamiento
        {
            Codigo = $"INT-{DateTime.UtcNow:yyyyMMddHHmmss}-{request.PokemonId}",
            PokemonId = request.PokemonId,
            UsuarioResponsableId = usuarioId,
            FechaIngresoUtc = request.FechaIngresoUtc,
            Motivo = request.Motivo.Trim(),
            AreaAsignada = request.AreaAsignada.Trim(),
            Estado = request.Estado
        };

        pokemon.EstadoSalud = EstadoPokemon.Hospitalizado;
        await _internamientoDA.CrearAsync(internamiento);
        await _pokemonDA.ActualizarAsync(pokemon);
        await _auditoriaBW.RegistrarAsync(usuarioId, "Internamiento", internamiento.Codigo, TipoOperacionAuditoria.Crear, "Internamiento registrado.");
        return internamiento;
    }

    public async Task<Internamiento?> DarAltaMedicaAsync(int usuarioId, int id, AltaMedicaRequest request)
    {
        var internamiento = await _internamientoDA.ObtenerPorIdAsync(id);
        if (internamiento is null)
        {
            return null;
        }

        var pendientes = await _tratamientoDA.ObtenerPendientesPorInternamientoAsync(internamiento.Id);
        if (pendientes.Any())
        {
            throw new InvalidOperationException("No se puede dar de alta mientras existan tratamientos pendientes.");
        }

        var pokemon = await _pokemonDA.ObtenerPorIdAsync(internamiento.PokemonId)
            ?? throw new InvalidOperationException("No existe el pokemon asociado al internamiento.");

        if (pokemon.EstadoSalud != EstadoPokemon.Estable && pokemon.EstadoSalud != EstadoPokemon.Recuperado)
        {
            throw new InvalidOperationException("El pokemon debe estar estable para recibir alta medica.");
        }

        internamiento.Estado = EstadoInternamiento.AltaMedica;
        await _internamientoDA.ActualizarAsync(internamiento);
        await _auditoriaBW.RegistrarAsync(usuarioId, "Internamiento", internamiento.Codigo, TipoOperacionAuditoria.Aprobar, $"Alta medica registrada. {request.Observacion}".Trim());
        return internamiento;
    }
}
