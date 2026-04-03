using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BC.Entidades;

public class Auditoria
{
    public int Id { get; set; }
    public int? UsuarioId { get; set; }
    public string Entidad { get; set; } = string.Empty;
    public string EntidadId { get; set; } = string.Empty;
    public TipoOperacionAuditoria Operacion { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaUtc { get; set; } = DateTime.UtcNow;

    public Usuario? Usuario { get; set; }
}
