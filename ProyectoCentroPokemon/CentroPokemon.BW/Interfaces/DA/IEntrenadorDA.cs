using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.DA;

public interface IEntrenadorDA
{
    Task<List<Entrenador>> ListarAsync();
    Task<Entrenador?> ObtenerPorIdAsync(int id);
    Task CrearAsync(Entrenador entrenador);
    Task ActualizarAsync(Entrenador entrenador);
    Task EliminarAsync(Entrenador entrenador);
}
