using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BC.Entidades;

public class Internamiento
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public int PokemonId { get; set; }
    public int UsuarioResponsableId { get; set; }
    public DateTime FechaIngresoUtc { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string AreaAsignada { get; set; } = string.Empty;
    public EstadoInternamiento Estado { get; set; } = EstadoInternamiento.Activo;

    public Pokemon? Pokemon { get; set; }
    public Usuario? UsuarioResponsable { get; set; }
}
