using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BC.DTOs.Pokemon;

public class PokemonRequest
{
    public string IdentificadorUnico { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public string TipoPrimario { get; set; } = string.Empty;
    public string? TipoSecundario { get; set; }
    public EstadoPokemon EstadoSalud { get; set; } = EstadoPokemon.Estable;
    public int EntrenadorId { get; set; }
}
