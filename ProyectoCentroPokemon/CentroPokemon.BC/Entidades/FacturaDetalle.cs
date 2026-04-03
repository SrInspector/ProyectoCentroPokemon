namespace CentroPokemon.BC.Entidades;

public class FacturaDetalle
{
    public int Id { get; set; }
    public int FacturaClinicaId { get; set; }
    public int ServicioClinicoId { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
    public decimal Subtotal { get; set; }

    public FacturaClinica? FacturaClinica { get; set; }
    public ServicioClinico? ServicioClinico { get; set; }
}
