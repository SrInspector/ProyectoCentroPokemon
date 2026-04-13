using CentroPokemon.BC.Entidades;
using CentroPokemon.BW.Interfaces.DA;
using CentroPokemon.DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CentroPokemon.DA.Repositorios;

public class CitaDA : ICitaDA
{
    private readonly CentroPokemonDbContext _context;

    public CitaDA(CentroPokemonDbContext context)
    {
        _context = context;
    }

    public Task<List<Cita>> ListarAsync(int? entrenadorId = null)
        => _context.Citas
            .Include(x => x.Entrenador)
            .ThenInclude(x => x!.Usuarios)
            .Include(x => x.Pokemon)
            .Include(x => x.UsuarioAsignado)
            .Include(x => x.Atencion)
            .Where(x => !entrenadorId.HasValue || x.EntrenadorId == entrenadorId.Value)
            .OrderBy(x => x.FechaProgramadaUtc)
            .ToListAsync();

    public Task<Cita?> ObtenerPorIdAsync(int id)
        => _context.Citas
            .Include(x => x.Entrenador)
            .ThenInclude(x => x!.Usuarios)
            .Include(x => x.Pokemon)
            .Include(x => x.UsuarioAsignado)
            .Include(x => x.Atencion)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task CrearAsync(Cita cita)
    {
        _context.Citas.Add(cita);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Cita cita)
    {
        _context.Citas.Update(cita);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Cita cita)
    {
        _context.Citas.Remove(cita);
        await _context.SaveChangesAsync();
    }
}
