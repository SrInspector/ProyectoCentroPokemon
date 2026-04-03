using CentroPokemon.BC.DTOs.Inventario;
using CentroPokemon.BC.Entidades;
using CentroPokemon.BW.Interfaces.BW;
using CentroPokemon.BW.Interfaces.DA;

namespace CentroPokemon.BW.CU;

public class GestionarInventarioBW : IGestionarInventarioBW
{
    private readonly IInventarioDA _inventarioDA;

    public GestionarInventarioBW(IInventarioDA inventarioDA)
    {
        _inventarioDA = inventarioDA;
    }

    public Task<List<ItemInventario>> ListarAsync() => _inventarioDA.ListarAsync();

    public Task<ItemInventario?> ObtenerPorIdAsync(int id) => _inventarioDA.ObtenerPorIdAsync(id);

    public async Task<ItemInventario> CrearAsync(ItemInventarioRequest request)
    {
        Validar(request);
        var item = new ItemInventario
        {
            Nombre = request.Nombre.Trim(),
            Categoria = request.Categoria.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim(),
            Stock = request.Stock,
            StockMinimo = request.StockMinimo
        };

        await _inventarioDA.CrearAsync(item);
        return item;
    }

    public async Task<ItemInventario?> ActualizarAsync(int id, ItemInventarioRequest request)
    {
        Validar(request);
        var item = await _inventarioDA.ObtenerPorIdAsync(id);
        if (item is null)
        {
            return null;
        }

        item.Nombre = request.Nombre.Trim();
        item.Categoria = request.Categoria.Trim();
        item.Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim();
        item.Stock = request.Stock;
        item.StockMinimo = request.StockMinimo;
        item.FechaActualizacionUtc = DateTime.UtcNow;

        await _inventarioDA.ActualizarAsync(item);
        return item;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var item = await _inventarioDA.ObtenerPorIdAsync(id);
        if (item is null)
        {
            return false;
        }

        await _inventarioDA.EliminarAsync(item);
        return true;
    }

    private static void Validar(ItemInventarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Categoria))
        {
            throw new InvalidOperationException("El nombre y la categoria del item son requeridos.");
        }

        if (request.Stock < 0 || request.StockMinimo < 0)
        {
            throw new InvalidOperationException("Los valores de stock no pueden ser negativos.");
        }
    }
}
