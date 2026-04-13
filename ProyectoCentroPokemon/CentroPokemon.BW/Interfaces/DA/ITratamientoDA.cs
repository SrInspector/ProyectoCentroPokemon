using CentroPokemon.BC.Entidades;
using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BW.Interfaces.DA;

public interface ITratamientoDA
{
    Task<List<TratamientoMedico>> ListarAsync(int? entrenadorId = null);
    Task<TratamientoMedico?> ObtenerPorIdAsync(int id);
    Task<List<TratamientoMedico>> ObtenerPorPokemonAsync(int pokemonId);
    Task<List<TratamientoMedico>> FiltrarAsync(int? entrenadorId, string? tipo, EstadoTratamiento? estado, DateTime? fechaInicioUtc, DateTime? fechaFinUtc);
    Task<List<TratamientoMedico>> ObtenerPendientesPorInternamientoAsync(int internamientoId);
    Task CrearAsync(TratamientoMedico tratamiento);
    Task ActualizarAsync(TratamientoMedico tratamiento);
}
