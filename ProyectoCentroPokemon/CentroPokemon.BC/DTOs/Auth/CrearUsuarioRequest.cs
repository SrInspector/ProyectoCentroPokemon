using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BC.DTOs.Auth;

public class CrearUsuarioRequest
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public RolSistema Rol { get; set; } = RolSistema.Recepcionista;
    public int? EntrenadorId { get; set; }
}
