using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BC.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public RolSistema Rol { get; set; }
}
