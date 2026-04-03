using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BC.DTOs.Internamientos;

public class InternamientoRequest
{
    public int PokemonId { get; set; }
    public DateTime FechaIngresoUtc { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string AreaAsignada { get; set; } = string.Empty;
    public EstadoInternamiento Estado { get; set; } = EstadoInternamiento.Activo;
}
