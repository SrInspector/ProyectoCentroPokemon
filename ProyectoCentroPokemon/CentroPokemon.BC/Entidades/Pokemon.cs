using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BC.Entidades;

public class Pokemon
{
    public int Id { get; set; }
    public string IdentificadorUnico { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public string TipoPrimario { get; set; } = string.Empty;
    public string? TipoSecundario { get; set; }
    public EstadoPokemon EstadoSalud { get; set; } = EstadoPokemon.Estable;
    public int EntrenadorId { get; set; }

    public Entrenador? Entrenador { get; set; }
    public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    public ICollection<Atencion> Atenciones { get; set; } = new List<Atencion>();
    public ICollection<Internamiento> Internamientos { get; set; } = new List<Internamiento>();
    public ICollection<TratamientoMedico> Tratamientos { get; set; } = new List<TratamientoMedico>();
}
