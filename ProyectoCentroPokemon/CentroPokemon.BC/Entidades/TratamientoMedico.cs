using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BC.Entidades;

public class TratamientoMedico
{
    public int Id { get; set; }
    public int PokemonId { get; set; }
    public int UsuarioResponsableId { get; set; }
    public int? InternamientoId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Dosis { get; set; } = string.Empty;
    public string Frecuencia { get; set; } = string.Empty;
    public DateTime FechaInicioUtc { get; set; }
    public DateTime FechaFinUtc { get; set; }
    public bool EsCritico { get; set; }
    public EstadoTratamiento Estado { get; set; } = EstadoTratamiento.Programado;

    public Pokemon? Pokemon { get; set; }
    public Usuario? UsuarioResponsable { get; set; }
    public Internamiento? Internamiento { get; set; }
}
