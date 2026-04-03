namespace CentroPokemon.BC.DTOs.Atenciones;

public class AtencionRequest
{
    public int CitaId { get; set; }
    public string Diagnostico { get; set; } = string.Empty;
    public string Tratamiento { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
}
