namespace CentroPokemon.BC.DTOs.Entrenadores;

public class EntrenadorRequest
{
    public string Identificacion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
}
