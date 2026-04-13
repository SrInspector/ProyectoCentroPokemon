using CentroPokemon.BC.DTOs.Tratamientos;
using CentroPokemon.BC.Entidades;
using CentroPokemon.BC.Enumeradores;
using CentroPokemon.BW.Interfaces.BW;
using CentroPokemon.BW.Interfaces.DA;

namespace CentroPokemon.BW.CU;

public class GestionarTratamientosBW : IGestionarTratamientosBW
{
    private readonly ITratamientoDA _tratamientoDA;
    private readonly IPokemonDA _pokemonDA;
    private readonly IInternamientoDA _internamientoDA;
    private readonly IUsuarioDA _usuarioDA;
    private readonly IAuditoriaBW _auditoriaBW;

    public GestionarTratamientosBW(ITratamientoDA tratamientoDA, IPokemonDA pokemonDA, IInternamientoDA internamientoDA, IUsuarioDA usuarioDA, IAuditoriaBW auditoriaBW)
    {
        _tratamientoDA = tratamientoDA;
        _pokemonDA = pokemonDA;
        _internamientoDA = internamientoDA;
        _usuarioDA = usuarioDA;
        _auditoriaBW = auditoriaBW;
    }

    public async Task<List<TratamientoMedico>> ListarAsync(int usuarioId, string rol, string? tipo, EstadoTratamiento? estado, DateTime? fechaInicioUtc, DateTime? fechaFinUtc)
    {
        int? entrenadorId = null;
        if (rol == RolSistema.Entrenador.ToString())
        {
            var usuario = await _usuarioDA.ObtenerPorIdAsync(usuarioId);
            if (usuario == null || !usuario.EntrenadorId.HasValue) return new List<TratamientoMedico>();
            entrenadorId = usuario.EntrenadorId.Value;
        }

        return await _tratamientoDA.FiltrarAsync(entrenadorId, tipo, estado, fechaInicioUtc, fechaFinUtc);
    }

    public async Task<TratamientoMedico?> ObtenerPorIdAsync(int usuarioId, string rol, int id)
    {
        var item = await _tratamientoDA.ObtenerPorIdAsync(id);
        if (item is null) return null;

        if (rol == RolSistema.Entrenador.ToString())
        {
            var usuario = await _usuarioDA.ObtenerPorIdAsync(usuarioId);
            if (usuario?.EntrenadorId != item.Pokemon?.EntrenadorId) return null;
        }

        return item;
    }

    public async Task<TratamientoMedico> CrearAsync(int usuarioId, TratamientoRequest request)
    {
        await ValidarTratamientoAsync(request);

        var tratamiento = new TratamientoMedico
        {
            PokemonId = request.PokemonId,
            UsuarioResponsableId = usuarioId,
            InternamientoId = request.InternamientoId,
            Tipo = request.Tipo.Trim(),
            Dosis = request.Dosis.Trim(),
            Frecuencia = request.Frecuencia.Trim(),
            FechaInicioUtc = request.FechaInicioUtc,
            FechaFinUtc = request.FechaFinUtc,
            EsCritico = request.EsCritico,
            Estado = request.Estado
        };

        await _tratamientoDA.CrearAsync(tratamiento);
        await _auditoriaBW.RegistrarAsync(usuarioId, "TratamientoMedico", tratamiento.Id.ToString(), TipoOperacionAuditoria.Crear, "Tratamiento registrado.");
        return tratamiento;
    }

    public async Task<TratamientoMedico?> ActualizarAsync(int usuarioId, int id, TratamientoRequest request)
    {
        await ValidarTratamientoAsync(request);
        var tratamiento = await _tratamientoDA.ObtenerPorIdAsync(id);
        if (tratamiento is null)
        {
            return null;
        }

        if (tratamiento.FechaInicioUtc <= DateTime.UtcNow || tratamiento.EsCritico)
        {
            throw new InvalidOperationException("Solo se puede editar un tratamiento que no haya iniciado y que no sea critico.");
        }

        tratamiento.Tipo = request.Tipo.Trim();
        tratamiento.Dosis = request.Dosis.Trim();
        tratamiento.Frecuencia = request.Frecuencia.Trim();
        tratamiento.FechaInicioUtc = request.FechaInicioUtc;
        tratamiento.FechaFinUtc = request.FechaFinUtc;
        tratamiento.EsCritico = request.EsCritico;
        tratamiento.Estado = request.Estado;
        tratamiento.InternamientoId = request.InternamientoId;

        await _tratamientoDA.ActualizarAsync(tratamiento);
        await _auditoriaBW.RegistrarAsync(usuarioId, "TratamientoMedico", tratamiento.Id.ToString(), TipoOperacionAuditoria.Actualizar, "Tratamiento actualizado.");
        return tratamiento;
    }

    public async Task<TratamientoMedico?> CambiarEstadoAsync(int usuarioId, int id, ActualizarEstadoTratamientoRequest request)
    {
        var tratamiento = await _tratamientoDA.ObtenerPorIdAsync(id);
        if (tratamiento is null)
        {
            return null;
        }

        if (request.Estado == EstadoTratamiento.Cancelado && (tratamiento.FechaInicioUtc <= DateTime.UtcNow || tratamiento.EsCritico))
        {
            throw new InvalidOperationException("No se puede cancelar un tratamiento ya iniciado o critico.");
        }

        tratamiento.Estado = request.Estado;
        await _tratamientoDA.ActualizarAsync(tratamiento);
        await _auditoriaBW.RegistrarAsync(usuarioId, "TratamientoMedico", tratamiento.Id.ToString(), TipoOperacionAuditoria.Actualizar, $"Cambio de estado a {request.Estado}.");
        return tratamiento;
    }

    private async Task ValidarTratamientoAsync(TratamientoRequest request)
    {
        var pokemon = await _pokemonDA.ObtenerPorIdAsync(request.PokemonId)
            ?? throw new InvalidOperationException("El pokemon indicado no existe.");

        if (string.IsNullOrWhiteSpace(request.Tipo) || string.IsNullOrWhiteSpace(request.Dosis) || string.IsNullOrWhiteSpace(request.Frecuencia))
        {
            throw new InvalidOperationException("Tipo, dosis y frecuencia son obligatorios.");
        }

        if (request.FechaFinUtc < request.FechaInicioUtc)
        {
            throw new InvalidOperationException("La fecha final no puede ser menor que la fecha inicial.");
        }

        if (request.InternamientoId.HasValue)
        {
            var internamiento = await _internamientoDA.ObtenerPorIdAsync(request.InternamientoId.Value)
                ?? throw new InvalidOperationException("El internamiento asociado no existe.");

            if (internamiento.PokemonId != pokemon.Id)
            {
                throw new InvalidOperationException("El internamiento no corresponde al pokemon indicado.");
            }
        }

        var existentes = await _tratamientoDA.ObtenerPorPokemonAsync(request.PokemonId);
        var conflicto = existentes.Any(x =>
            x.Estado != EstadoTratamiento.Cancelado &&
            x.Tipo.Equals(request.Tipo.Trim(), StringComparison.OrdinalIgnoreCase) &&
            request.FechaInicioUtc <= x.FechaFinUtc &&
            request.FechaFinUtc >= x.FechaInicioUtc);

        if (conflicto)
        {
            throw new InvalidOperationException("Existe conflicto con otro tratamiento del mismo tipo en el rango indicado.");
        }
    }
}
