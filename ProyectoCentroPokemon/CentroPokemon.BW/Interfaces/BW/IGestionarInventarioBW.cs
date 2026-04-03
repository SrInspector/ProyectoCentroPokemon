using CentroPokemon.BC.DTOs.Inventario;
using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.BW;

public interface IGestionarInventarioBW
{
    Task<List<ItemInventario>> ListarAsync();
    Task<ItemInventario?> ObtenerPorIdAsync(int id);
    Task<ItemInventario> CrearAsync(ItemInventarioRequest request);
    Task<ItemInventario?> ActualizarAsync(int id, ItemInventarioRequest request);
    Task<bool> EliminarAsync(int id);
}
