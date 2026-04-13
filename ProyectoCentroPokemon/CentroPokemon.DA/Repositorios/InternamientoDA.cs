using CentroPokemon.BC.Entidades;
using CentroPokemon.BC.Enumeradores;
using CentroPokemon.BW.Interfaces.DA;
using CentroPokemon.DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CentroPokemon.DA.Repositorios;

public class InternamientoDA : IInternamientoDA
{
    private readonly CentroPokemonDbContext _context;

    public InternamientoDA(CentroPokemonDbContext context)
    {
        _context = context;
    }

    public Task<List<Internamiento>> ListarAsync(int? entrenadorId = null)
        => _context.Internamientos
            .Include(x => x.Pokemon)
            .ThenInclude(x => x!.Entrenador)
            .ThenInclude(x => x!.Usuarios)
            .Include(x => x.UsuarioResponsable)
            .Where(x => !entrenadorId.HasValue || x.Pokemon!.EntrenadorId == entrenadorId.Value)
            .OrderByDescending(x => x.FechaIngresoUtc)
            .ToListAsync();

    public Task<Internamiento?> ObtenerPorIdAsync(int id)
        => _context.Internamientos
            .Include(x => x.Pokemon)
            .ThenInclude(x => x!.Entrenador)
            .ThenInclude(x => x!.Usuarios)
            .Include(x => x.UsuarioResponsable)
            .FirstOrDefaultAsync(x => x.Id == id);

    public Task<Internamiento?> ObtenerActivoPorPokemonAsync(int pokemonId)
        => _context.Internamientos.FirstOrDefaultAsync(x => x.PokemonId == pokemonId && (x.Estado == EstadoInternamiento.Activo || x.Estado == EstadoInternamiento.EnObservacion));

    public Task<List<Internamiento>> FiltrarAsync(int? entrenadorId, string? area, EstadoInternamiento? estado, DateTime? fechaInicioUtc, DateTime? fechaFinUtc)
    {
        var query = _context.Internamientos
            .Include(x => x.Pokemon)
            .ThenInclude(x => x!.Entrenador)
            .ThenInclude(x => x!.Usuarios)
            .Include(x => x.UsuarioResponsable)
            .AsQueryable();

        if (entrenadorId.HasValue)
        {
            query = query.Where(x => x.Pokemon!.EntrenadorId == entrenadorId.Value);
        }

        if (!string.IsNullOrWhiteSpace(area))
        {
            query = query.Where(x => x.AreaAsignada.Contains(area));
        }

        if (estado.HasValue)
        {
            query = query.Where(x => x.Estado == estado.Value);
        }

        if (fechaInicioUtc.HasValue)
        {
            query = query.Where(x => x.FechaIngresoUtc >= fechaInicioUtc.Value);
        }

        if (fechaFinUtc.HasValue)
        {
            query = query.Where(x => x.FechaIngresoUtc <= fechaFinUtc.Value);
        }

        return query.OrderByDescending(x => x.FechaIngresoUtc).ToListAsync();
    }

    public async Task CrearAsync(Internamiento internamiento)
    {
        _context.Internamientos.Add(internamiento);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Internamiento internamiento)
    {
        _context.Internamientos.Update(internamiento);
        await _context.SaveChangesAsync();
    }
}
