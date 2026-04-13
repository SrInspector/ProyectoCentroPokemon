using CentroPokemon.BC.DTOs.Citas;
using CentroPokemon.BC.Entidades;
using CentroPokemon.BW.Interfaces.BW;
using CentroPokemon.BW.Interfaces.DA;

namespace CentroPokemon.BW.CU;

public class GestionarCitasBW : IGestionarCitasBW
{
    private readonly ICitaDA _citaDA;
    private readonly IEntrenadorDA _entrenadorDA;
    private readonly IPokemonDA _pokemonDA;
    private readonly IUsuarioDA _usuarioDA;
    private readonly IAuditoriaBW _auditoriaBW;

    public GestionarCitasBW(ICitaDA citaDA, IEntrenadorDA entrenadorDA, IPokemonDA pokemonDA, IUsuarioDA usuarioDA, IAuditoriaBW auditoriaBW)
    {
        _citaDA = citaDA;
        _entrenadorDA = entrenadorDA;
        _pokemonDA = pokemonDA;
        _usuarioDA = usuarioDA;
        _auditoriaBW = auditoriaBW;
    }

    public Task<List<Cita>> ListarAsync(int? entrenadorId = null) => _citaDA.ListarAsync(entrenadorId);

    public Task<Cita?> ObtenerPorIdAsync(int id) => _citaDA.ObtenerPorIdAsync(id);

    public async Task<Cita> CrearAsync(CitaRequest request)
    {
        await ValidarAsync(request);
        var cita = new Cita
        {
            FechaProgramadaUtc = request.FechaProgramadaUtc,
            Motivo = request.Motivo.Trim(),
            EntrenadorId = request.EntrenadorId,
            PokemonId = request.PokemonId,
            UsuarioAsignadoId = request.UsuarioAsignadoId,
            RequiereAprobacionAdministrador = request.Motivo.Contains("cirugia", StringComparison.OrdinalIgnoreCase) || request.Motivo.Contains("compleja", StringComparison.OrdinalIgnoreCase)
        };

        await _citaDA.CrearAsync(cita);
        await _auditoriaBW.RegistrarAsync(request.UsuarioAsignadoId, "Cita", cita.Id.ToString(), BC.Enumeradores.TipoOperacionAuditoria.Crear, "Cita registrada.");
        return cita;
    }

    public async Task<Cita?> ActualizarAsync(int id, CitaRequest request)
    {
        await ValidarAsync(request);
        var cita = await _citaDA.ObtenerPorIdAsync(id);
        if (cita is null)
        {
            return null;
        }

        cita.FechaProgramadaUtc = request.FechaProgramadaUtc;
        cita.Motivo = request.Motivo.Trim();
        cita.EntrenadorId = request.EntrenadorId;
        cita.PokemonId = request.PokemonId;
        cita.UsuarioAsignadoId = request.UsuarioAsignadoId;

        await _citaDA.ActualizarAsync(cita);
        await _auditoriaBW.RegistrarAsync(request.UsuarioAsignadoId, "Cita", cita.Id.ToString(), BC.Enumeradores.TipoOperacionAuditoria.Actualizar, "Cita actualizada.");
        return cita;
    }

    public async Task<Cita?> ActualizarEstadoAsync(int id, ActualizarEstadoCitaRequest request)
    {
        var cita = await _citaDA.ObtenerPorIdAsync(id);
        if (cita is null)
        {
            return null;
        }

        cita.Estado = request.Estado;
        await _citaDA.ActualizarAsync(cita);
        await _auditoriaBW.RegistrarAsync(cita.UsuarioAsignadoId, "Cita", cita.Id.ToString(), BC.Enumeradores.TipoOperacionAuditoria.Actualizar, $"Cambio de estado a {request.Estado}.");
        return cita;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var cita = await _citaDA.ObtenerPorIdAsync(id);
        if (cita is null)
        {
            return false;
        }

        await _citaDA.EliminarAsync(cita);
        await _auditoriaBW.RegistrarAsync(cita.UsuarioAsignadoId, "Cita", cita.Id.ToString(), BC.Enumeradores.TipoOperacionAuditoria.Eliminar, "Cita eliminada.");
        return true;
    }

    private async Task ValidarAsync(CitaRequest request)
    {
        if (request.FechaProgramadaUtc <= DateTime.UtcNow.AddMinutes(-1))
        {
            throw new InvalidOperationException("La fecha de la cita debe ser futura.");
        }

        if (request.FechaProgramadaUtc > DateTime.UtcNow.AddDays(90))
        {
            throw new InvalidOperationException("La cita no puede programarse con mas de 90 dias de anticipacion.");
        }

        if (string.IsNullOrWhiteSpace(request.Motivo))
        {
            throw new InvalidOperationException("El motivo de la cita es requerido.");
        }

        if (await _entrenadorDA.ObtenerPorIdAsync(request.EntrenadorId) is null)
        {
            throw new InvalidOperationException("El entrenador indicado no existe.");
        }

        var pokemon = await _pokemonDA.ObtenerPorIdAsync(request.PokemonId)
            ?? throw new InvalidOperationException("El pokemon indicado no existe.");

        if (pokemon.EntrenadorId != request.EntrenadorId)
        {
            throw new InvalidOperationException("El pokemon no pertenece al entrenador indicado.");
        }

        if (pokemon.EstadoSalud == BC.Enumeradores.EstadoPokemon.Hospitalizado)
        {
            throw new InvalidOperationException("No se puede programar cita mientras el pokemon este hospitalizado.");
        }

        var conflictos = await _citaDA.ListarAsync();
        if (conflictos.Any(x => x.PokemonId == request.PokemonId && x.Estado != BC.Enumeradores.EstadoCita.Cancelada && Math.Abs((x.FechaProgramadaUtc - request.FechaProgramadaUtc).TotalMinutes) < 60))
        {
            throw new InvalidOperationException("Existe conflicto con otra cita del pokemon en el horario indicado.");
        }

        if (request.UsuarioAsignadoId.HasValue && await _usuarioDA.ObtenerPorIdAsync(request.UsuarioAsignadoId.Value) is null)
        {
            throw new InvalidOperationException("El usuario asignado no existe.");
        }
    }
}
