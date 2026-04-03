using System.Text.Json.Serialization;

namespace CentroPokemon.BC.DTOs.PokeApi;

public class PokeApiPokemonDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("weight")]
    public int Weight { get; set; }

    [JsonPropertyName("base_experience")]
    public int? BaseExperience { get; set; }

    [JsonPropertyName("types")]
    public List<PokeApiTypeSlotDto> Types { get; set; } = new();

    [JsonPropertyName("sprites")]
    public PokeApiSpritesDto? Sprites { get; set; }
}
