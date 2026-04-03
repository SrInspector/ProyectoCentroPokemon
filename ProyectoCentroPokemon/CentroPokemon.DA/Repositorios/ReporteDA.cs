using CentroPokemon.BC.DTOs.Reportes;
using CentroPokemon.BW.Interfaces.DA;
using CentroPokemon.DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CentroPokemon.DA.Repositorios;

public class ReporteDA : IReporteDA
{
    private readonly CentroPokemonDbContext _context;

    public ReporteDA(CentroPokemonDbContext context)
    {
        _context = context;
    }

    public Task<List<ReporteAtencionesItem>> ObtenerAtencionesPorPeriodoAsync(DateTime? fechaInicioUtc, DateTime? fechaFinUtc)
    {
        var query = _context.Atenciones.AsQueryable();
        if (fechaInicioUtc.HasValue) query = query.Where(x => x.FechaAtencionUtc >= fechaInicioUtc.Value);
        if (fechaFinUtc.HasValue) query = query.Where(x => x.FechaAtencionUtc <= fechaFinUtc.Value);

        return query.GroupBy(x => x.FechaAtencionUtc.Date)
            .Select(g => new ReporteAtencionesItem { Fecha = g.Key, Cantidad = g.Count() })
            .OrderBy(x => x.Fecha)
            .ToListAsync();
    }

    public Task<List<ReporteTopPokemonItem>> ObtenerTopPokemonPorTratamientosAsync(DateTime? fechaInicioUtc, DateTime? fechaFinUtc)
    {
        var query = _context.TratamientosMedicos.Include(x => x.Pokemon).AsQueryable();
        if (fechaInicioUtc.HasValue) query = query.Where(x => x.FechaInicioUtc >= fechaInicioUtc.Value);
        if (fechaFinUtc.HasValue) query = query.Where(x => x.FechaFinUtc <= fechaFinUtc.Value);

        return query.GroupBy(x => x.Pokemon!.Nombre)
            .Select(g => new ReporteTopPokemonItem { Pokemon = g.Key, CantidadTratamientos = g.Count() })
            .OrderByDescending(x => x.CantidadTratamientos)
            .Take(10)
            .ToListAsync();
    }

    public Task<List<ReporteUsoAreaItem>> ObtenerUsoAreasClinicasAsync(DateTime? fechaInicioUtc, DateTime? fechaFinUtc)
    {
        var query = _context.Internamientos.AsQueryable();
        if (fechaInicioUtc.HasValue) query = query.Where(x => x.FechaIngresoUtc >= fechaInicioUtc.Value);
        if (fechaFinUtc.HasValue) query = query.Where(x => x.FechaIngresoUtc <= fechaFinUtc.Value);

        return query.GroupBy(x => x.AreaAsignada)
            .Select(g => new ReporteUsoAreaItem { AreaAsignada = g.Key, CantidadInternamientos = g.Count() })
            .OrderByDescending(x => x.CantidadInternamientos)
            .ToListAsync();
    }
}
