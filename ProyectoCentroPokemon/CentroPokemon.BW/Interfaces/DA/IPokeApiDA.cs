using CentroPokemon.BC.DTOs.PokeApi;

namespace CentroPokemon.BW.Interfaces.DA;

public interface IPokeApiDA
{
    Task<PokeApiPokemonDto?> ObtenerPokemonAsync(string nombreOId);
}
