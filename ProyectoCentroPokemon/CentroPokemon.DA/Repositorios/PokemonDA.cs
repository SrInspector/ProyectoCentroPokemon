using CentroPokemon.BC.Entidades;
using CentroPokemon.BW.Interfaces.DA;
using CentroPokemon.DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CentroPokemon.DA.Repositorios;

public class PokemonDA : IPokemonDA
{
    private readonly CentroPokemonDbContext _context;

    public PokemonDA(CentroPokemonDbContext context)
    {
        _context = context;
    }

    public Task<List<Pokemon>> ListarAsync()
        => _context.Pokemones.Include(x => x.Entrenador).ThenInclude(x => x!.Usuarios).OrderBy(x => x.Nombre).ToListAsync();

    public Task<Pokemon?> ObtenerPorIdAsync(int id)
        => _context.Pokemones
            .Include(x => x.Entrenador)
            .ThenInclude(x => x!.Usuarios)
            .Include(x => x.Citas)
            .Include(x => x.Atenciones)
            .Include(x => x.Internamientos)
            .Include(x => x.Tratamientos)
            .FirstOrDefaultAsync(x => x.Id == id);

    public Task<Pokemon?> ObtenerPorIdentificadorUnicoAsync(string identificadorUnico)
        => _context.Pokemones.FirstOrDefaultAsync(x => x.IdentificadorUnico == identificadorUnico);

    public async Task CrearAsync(Pokemon pokemon)
    {
        _context.Pokemones.Add(pokemon);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Pokemon pokemon)
    {
        _context.Pokemones.Update(pokemon);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Pokemon pokemon)
    {
        _context.Pokemones.Remove(pokemon);
        await _context.SaveChangesAsync();
    }
}
