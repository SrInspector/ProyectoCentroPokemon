using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BC.Entidades;

public class Cita
{
    public int Id { get; set; }
    public DateTime FechaProgramadaUtc { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public EstadoCita Estado { get; set; } = EstadoCita.Pendiente;
    public bool RequiereAprobacionAdministrador { get; set; }
    public bool AprobadaPorAdministrador { get; set; }
    public int EntrenadorId { get; set; }
    public int PokemonId { get; set; }
    public int? UsuarioAsignadoId { get; set; }

    public Entrenador? Entrenador { get; set; }
    public Pokemon? Pokemon { get; set; }
    public Usuario? UsuarioAsignado { get; set; }
    public Atencion? Atencion { get; set; }
}
