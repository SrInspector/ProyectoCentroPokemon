using CentroPokemon.BC.DTOs.Auth;

namespace CentroPokemon.BW.Interfaces.BW;

public interface IAutenticacionBW
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> CrearUsuarioAsync(CrearUsuarioRequest request);
}
