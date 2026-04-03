using CentroPokemon.BC.Entidades;
using CentroPokemon.BW.Interfaces.DA;
using CentroPokemon.DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CentroPokemon.DA.Repositorios;

public class FacturaDA : IFacturaDA
{
    private readonly CentroPokemonDbContext _context;

    public FacturaDA(CentroPokemonDbContext context)
    {
        _context = context;
    }

    public Task<List<FacturaClinica>> ListarAsync()
        => _context.FacturasClinicas
            .Include(x => x.Entrenador)
            .Include(x => x.UsuarioGenerador)
            .Include(x => x.Detalles)
            .ThenInclude(x => x.ServicioClinico)
            .OrderByDescending(x => x.FechaEmisionUtc)
            .ToListAsync();

    public Task<FacturaClinica?> ObtenerPorIdAsync(int id)
        => _context.FacturasClinicas
            .Include(x => x.Entrenador)
            .Include(x => x.UsuarioGenerador)
            .Include(x => x.Detalles)
            .ThenInclude(x => x.ServicioClinico)
            .FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<FacturaClinica>> ObtenerPorEntrenadorAsync(int entrenadorId)
        => _context.FacturasClinicas
            .Include(x => x.Entrenador)
            .Include(x => x.UsuarioGenerador)
            .Include(x => x.Detalles)
            .ThenInclude(x => x.ServicioClinico)
            .Where(x => x.EntrenadorId == entrenadorId)
            .OrderByDescending(x => x.FechaEmisionUtc)
            .ToListAsync();

    public async Task CrearAsync(FacturaClinica factura)
    {
        _context.FacturasClinicas.Add(factura);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(FacturaClinica factura)
    {
        _context.FacturasClinicas.Update(factura);
        await _context.SaveChangesAsync();
    }
}
