namespace CentroPokemon.BC.DTOs.Inventario;

public class ItemInventarioRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
}
