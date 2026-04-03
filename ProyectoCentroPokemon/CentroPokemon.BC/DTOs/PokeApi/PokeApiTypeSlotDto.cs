using System.Text.Json.Serialization;

namespace CentroPokemon.BC.DTOs.PokeApi;

public class PokeApiTypeSlotDto
{
    [JsonPropertyName("slot")]
    public int Slot { get; set; }

    [JsonPropertyName("type")]
    public PokeApiNamedResourceDto? Type { get; set; }
}
