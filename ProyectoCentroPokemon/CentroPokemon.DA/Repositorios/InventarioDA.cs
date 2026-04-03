using CentroPokemon.BC.Entidades;
using CentroPokemon.BW.Interfaces.DA;
using CentroPokemon.DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CentroPokemon.DA.Repositorios;

public class InventarioDA : IInventarioDA
{
    private readonly CentroPokemonDbContext _context;

    public InventarioDA(CentroPokemonDbContext context)
    {
        _context = context;
    }

    public Task<List<ItemInventario>> ListarAsync()
        => _context.Inventario.OrderBy(x => x.Nombre).ToListAsync();

    public Task<ItemInventario?> ObtenerPorIdAsync(int id)
        => _context.Inventario.FirstOrDefaultAsync(x => x.Id == id);

    public async Task CrearAsync(ItemInventario item)
    {
        _context.Inventario.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(ItemInventario item)
    {
        _context.Inventario.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(ItemInventario item)
    {
        _context.Inventario.Remove(item);
        await _context.SaveChangesAsync();
    }
}
