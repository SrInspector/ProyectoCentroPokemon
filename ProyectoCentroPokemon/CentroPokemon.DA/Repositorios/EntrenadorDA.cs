using CentroPokemon.BC.Entidades;
using CentroPokemon.BW.Interfaces.DA;
using CentroPokemon.DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CentroPokemon.DA.Repositorios;

public class EntrenadorDA : IEntrenadorDA
{
    private readonly CentroPokemonDbContext _context;

    public EntrenadorDA(CentroPokemonDbContext context)
    {
        _context = context;
    }

    public Task<List<Entrenador>> ListarAsync()
        => _context.Entrenadores.Include(x => x.Pokemones).Include(x => x.Usuarios).OrderBy(x => x.Nombre).ToListAsync();

    public Task<Entrenador?> ObtenerPorIdAsync(int id)
        => _context.Entrenadores.Include(x => x.Pokemones).Include(x => x.Usuarios).FirstOrDefaultAsync(x => x.Id == id);

    public async Task CrearAsync(Entrenador entrenador)
    {
        _context.Entrenadores.Add(entrenador);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Entrenador entrenador)
    {
        _context.Entrenadores.Update(entrenador);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Entrenador entrenador)
    {
        _context.Entrenadores.Remove(entrenador);
        await _context.SaveChangesAsync();
    }
}
