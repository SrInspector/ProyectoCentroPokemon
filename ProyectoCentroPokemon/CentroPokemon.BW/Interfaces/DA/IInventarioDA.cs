using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.DA;

public interface IInventarioDA
{
    Task<List<ItemInventario>> ListarAsync();
    Task<ItemInventario?> ObtenerPorIdAsync(int id);
    Task CrearAsync(ItemInventario item);
    Task ActualizarAsync(ItemInventario item);
    Task EliminarAsync(ItemInventario item);
}
