using CentroPokemon.BC.Entidades;
using CentroPokemon.BC.Enumeradores;
using CentroPokemon.BW.Interfaces.DA;
using CentroPokemon.DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CentroPokemon.DA.Repositorios;

public class TratamientoDA : ITratamientoDA
{
    private readonly CentroPokemonDbContext _context;

    public TratamientoDA(CentroPokemonDbContext context)
    {
        _context = context;
    }

    public Task<List<TratamientoMedico>> ListarAsync(int? entrenadorId = null)
        => _context.TratamientosMedicos
            .Include(x => x.Pokemon)
            .ThenInclude(x => x!.Entrenador)
            .ThenInclude(x => x!.Usuarios)
            .Include(x => x.UsuarioResponsable)
            .Include(x => x.Internamiento)
            .Where(x => !entrenadorId.HasValue || x.Pokemon!.EntrenadorId == entrenadorId.Value)
            .OrderByDescending(x => x.FechaInicioUtc)
            .ToListAsync();

    public Task<TratamientoMedico?> ObtenerPorIdAsync(int id)
        => _context.TratamientosMedicos
            .Include(x => x.Pokemon)
            .ThenInclude(x => x!.Entrenador)
            .ThenInclude(x => x!.Usuarios)
            .Include(x => x.UsuarioResponsable)
            .Include(x => x.Internamiento)
            .FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<TratamientoMedico>> ObtenerPorPokemonAsync(int pokemonId)
        => _context.TratamientosMedicos.Where(x => x.PokemonId == pokemonId).ToListAsync();

    public Task<List<TratamientoMedico>> FiltrarAsync(int? entrenadorId, string? tipo, EstadoTratamiento? estado, DateTime? fechaInicioUtc, DateTime? fechaFinUtc)
    {
        var query = _context.TratamientosMedicos
            .Include(x => x.Pokemon)
            .ThenInclude(x => x!.Entrenador)
            .ThenInclude(x => x!.Usuarios)
            .Include(x => x.UsuarioResponsable)
            .Include(x => x.Internamiento)
            .AsQueryable();

        if (entrenadorId.HasValue)
        {
            query = query.Where(x => x.Pokemon!.EntrenadorId == entrenadorId.Value);
        }

        if (!string.IsNullOrWhiteSpace(tipo))
        {
            query = query.Where(x => x.Tipo.Contains(tipo));
        }

        if (estado.HasValue)
        {
            query = query.Where(x => x.Estado == estado.Value);
        }

        if (fechaInicioUtc.HasValue)
        {
            query = query.Where(x => x.FechaInicioUtc >= fechaInicioUtc.Value);
        }

        if (fechaFinUtc.HasValue)
        {
            query = query.Where(x => x.FechaFinUtc <= fechaFinUtc.Value);
        }

        return query.OrderByDescending(x => x.FechaInicioUtc).ToListAsync();
    }

    public Task<List<TratamientoMedico>> ObtenerPendientesPorInternamientoAsync(int internamientoId)
        => _context.TratamientosMedicos.Where(x => x.InternamientoId == internamientoId && x.Estado != EstadoTratamiento.Finalizado && x.Estado != EstadoTratamiento.Cancelado).ToListAsync();

    public async Task CrearAsync(TratamientoMedico tratamiento)
    {
        _context.TratamientosMedicos.Add(tratamiento);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(TratamientoMedico tratamiento)
    {
        _context.TratamientosMedicos.Update(tratamiento);
        await _context.SaveChangesAsync();
    }
}
