namespace CentroPokemon.BC.Entidades;

public class Atencion
{
    public int Id { get; set; }
    public int CitaId { get; set; }
    public int PokemonId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime FechaAtencionUtc { get; set; } = DateTime.UtcNow;
    public string Diagnostico { get; set; } = string.Empty;
    public string Tratamiento { get; set; } = string.Empty;
    public string? Observaciones { get; set; }

    public Cita? Cita { get; set; }
    public Pokemon? Pokemon { get; set; }
    public Usuario? Usuario { get; set; }
}
