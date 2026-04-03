using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BC.DTOs.Tratamientos;

public class TratamientoRequest
{
    public int PokemonId { get; set; }
    public int? InternamientoId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Dosis { get; set; } = string.Empty;
    public string Frecuencia { get; set; } = string.Empty;
    public DateTime FechaInicioUtc { get; set; }
    public DateTime FechaFinUtc { get; set; }
    public bool EsCritico { get; set; }
    public EstadoTratamiento Estado { get; set; } = EstadoTratamiento.Programado;
}
