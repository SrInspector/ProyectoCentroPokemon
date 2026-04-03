namespace CentroPokemon.BC.DTOs.Facturacion;

public class ServicioClinicoRequest
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal CostoBase { get; set; }
    public bool RequiereAprobacionAdministrador { get; set; }
    public bool EsRecurrente { get; set; }
}
