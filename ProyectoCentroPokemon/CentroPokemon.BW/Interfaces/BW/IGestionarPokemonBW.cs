using CentroPokemon.BC.DTOs.Pokemon;
using CentroPokemon.BC.DTOs.PokeApi;
using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.BW;

public interface IGestionarPokemonBW
{
    Task<List<Pokemon>> ListarAsync();
    Task<Pokemon?> ObtenerPorIdAsync(int id);
    Task<PokeApiPokemonDto?> BuscarEnPokeApiAsync(string nombreOId);
    Task<Pokemon> ImportarDesdePokeApiAsync(PokemonImportRequest request);
    Task<Pokemon> CrearAsync(PokemonRequest request);
    Task<Pokemon?> ActualizarAsync(int id, PokemonRequest request);
    Task<bool> EliminarAsync(int id);
}
