using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.DA;

public interface IPokemonDA
{
    Task<List<Pokemon>> ListarAsync(int? entrenadorId = null);
    Task<Pokemon?> ObtenerPorIdAsync(int id);
    Task<Pokemon?> ObtenerPorIdentificadorUnicoAsync(string identificadorUnico);
    Task CrearAsync(Pokemon pokemon);
    Task ActualizarAsync(Pokemon pokemon);
    Task EliminarAsync(Pokemon pokemon);
}
