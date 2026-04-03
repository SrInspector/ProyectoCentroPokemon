namespace CentroPokemon.BC.DTOs.Citas;

public class CitaRequest
{
    public DateTime FechaProgramadaUtc { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public int EntrenadorId { get; set; }
    public int PokemonId { get; set; }
    public int? UsuarioAsignadoId { get; set; }
}
