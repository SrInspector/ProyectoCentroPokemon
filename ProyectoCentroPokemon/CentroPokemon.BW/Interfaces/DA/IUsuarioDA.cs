using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.DA;

public interface IUsuarioDA
{
    Task<Usuario?> ObtenerPorCorreoAsync(string correo);
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<List<Usuario>> ListarAsync();
    Task CrearAsync(Usuario usuario);
    Task ActualizarAsync(Usuario usuario);
}
