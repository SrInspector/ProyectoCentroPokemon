using System.Text.Json.Serialization;

namespace CentroPokemon.BC.DTOs.PokeApi;

public class PokeApiSpritesDto
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; set; }

    [JsonPropertyName("front_shiny")]
    public string? FrontShiny { get; set; }
}
