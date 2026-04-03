using System.Net.Http.Json;
using CentroPokemon.BC.DTOs.PokeApi;
using CentroPokemon.BW.Interfaces.DA;

namespace CentroPokemon.DA.Servicios;

public class PokeApiDA : IPokeApiDA
{
    private readonly HttpClient _httpClient;

    public PokeApiDA(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PokeApiPokemonDto?> ObtenerPokemonAsync(string nombreOId)
    {
        if (string.IsNullOrWhiteSpace(nombreOId))
        {
            return null;
        }

        try
        {
            return await _httpClient.GetFromJsonAsync<PokeApiPokemonDto>($"pokemon/{nombreOId.Trim().ToLowerInvariant()}");
        }
        catch
        {
            return null;
        }
    }
}
