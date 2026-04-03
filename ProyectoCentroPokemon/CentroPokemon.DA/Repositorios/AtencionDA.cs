using CentroPokemon.BC.Entidades;
using CentroPokemon.BW.Interfaces.DA;
using CentroPokemon.DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CentroPokemon.DA.Repositorios;

public class AtencionDA : IAtencionDA
{
    private readonly CentroPokemonDbContext _context;

    public AtencionDA(CentroPokemonDbContext context)
    {
        _context = context;
    }

    public Task<List<Atencion>> ListarAsync()
        => _context.Atenciones
            .Include(x => x.Cita)
            .Include(x => x.Pokemon)
            .ThenInclude(x => x!.Entrenador)
            .Include(x => x.Usuario)
            .OrderByDescending(x => x.FechaAtencionUtc)
            .ToListAsync();

    public Task<Atencion?> ObtenerPorIdAsync(int id)
        => _context.Atenciones
            .Include(x => x.Cita)
            .Include(x => x.Pokemon)
            .ThenInclude(x => x!.Entrenador)
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task CrearAsync(Atencion atencion)
    {
        _context.Atenciones.Add(atencion);
        await _context.SaveChangesAsync();
    }
}
