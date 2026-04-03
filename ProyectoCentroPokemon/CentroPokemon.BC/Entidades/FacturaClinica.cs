using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BC.Entidades;

public class FacturaClinica
{
    public int Id { get; set; }
    public string Referencia { get; set; } = string.Empty;
    public int EntrenadorId { get; set; }
    public int UsuarioGeneradorId { get; set; }
    public DateTime FechaEmisionUtc { get; set; } = DateTime.UtcNow;
    public EstadoFactura Estado { get; set; } = EstadoFactura.Pendiente;
    public decimal Total { get; set; }
    public string? RutaComprobante { get; set; }

    public Entrenador? Entrenador { get; set; }
    public Usuario? UsuarioGenerador { get; set; }
    public ICollection<FacturaDetalle> Detalles { get; set; } = new List<FacturaDetalle>();
}
