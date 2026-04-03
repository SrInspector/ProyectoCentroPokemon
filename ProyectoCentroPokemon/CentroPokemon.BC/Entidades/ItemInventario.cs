namespace CentroPokemon.BC.Entidades;

public class ItemInventario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public DateTime FechaActualizacionUtc { get; set; } = DateTime.UtcNow;
}
