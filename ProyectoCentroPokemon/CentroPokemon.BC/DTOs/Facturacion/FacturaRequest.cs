namespace CentroPokemon.BC.DTOs.Facturacion;

public class FacturaRequest
{
    public int EntrenadorId { get; set; }
    public List<FacturaDetalleRequest> Detalles { get; set; } = new();
}
