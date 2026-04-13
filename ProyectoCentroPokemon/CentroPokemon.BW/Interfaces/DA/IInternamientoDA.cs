using CentroPokemon.BC.Entidades;
using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BW.Interfaces.DA;

public interface IInternamientoDA
{
    Task<List<Internamiento>> ListarAsync(int? entrenadorId = null);
    Task<Internamiento?> ObtenerPorIdAsync(int id);
    Task<Internamiento?> ObtenerActivoPorPokemonAsync(int pokemonId);
    Task<List<Internamiento>> FiltrarAsync(int? entrenadorId, string? area, EstadoInternamiento? estado, DateTime? fechaInicioUtc, DateTime? fechaFinUtc);
    Task CrearAsync(Internamiento internamiento);
    Task ActualizarAsync(Internamiento internamiento);
}
