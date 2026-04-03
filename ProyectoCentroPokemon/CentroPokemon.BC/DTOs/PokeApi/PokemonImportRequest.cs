namespace CentroPokemon.BC.DTOs.PokeApi;

public class PokemonImportRequest
{
    public string NombreOId { get; set; } = string.Empty;
    public string IdentificadorUnico { get; set; } = string.Empty;
    public string NombreAsignado { get; set; } = string.Empty;
    public int Nivel { get; set; } = 1;
    public int EntrenadorId { get; set; }
}
