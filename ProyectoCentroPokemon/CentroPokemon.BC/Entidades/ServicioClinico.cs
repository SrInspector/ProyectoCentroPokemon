namespace CentroPokemon.BC.Entidades;

public class ServicioClinico
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal CostoBase { get; set; }
    public bool RequiereAprobacionAdministrador { get; set; }
    public bool EsRecurrente { get; set; }

    public ICollection<FacturaDetalle> FacturasDetalle { get; set; } = new List<FacturaDetalle>();
}
