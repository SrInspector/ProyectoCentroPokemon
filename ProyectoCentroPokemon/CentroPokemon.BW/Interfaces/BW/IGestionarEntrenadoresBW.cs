using CentroPokemon.BC.DTOs.Entrenadores;
using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.BW;

public interface IGestionarEntrenadoresBW
{
    Task<List<Entrenador>> ListarAsync();
    Task<Entrenador?> ObtenerPorIdAsync(int id);
    Task<Entrenador> CrearAsync(EntrenadorRequest request);
    Task<Entrenador?> ActualizarAsync(int id, EntrenadorRequest request);
    Task<bool> EliminarAsync(int id);
}
