namespace CentroPokemon.BC.Entidades;

public class Entrenador
{
    public int Id { get; set; }
    public string Identificacion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;

    public ICollection<Pokemon> Pokemones { get; set; } = new List<Pokemon>();
    public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public ICollection<FacturaClinica> Facturas { get; set; } = new List<FacturaClinica>();
}
