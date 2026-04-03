using System.Text.Json.Serialization;

namespace CentroPokemon.BC.DTOs.PokeApi;

public class PokeApiNamedResourceDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}
