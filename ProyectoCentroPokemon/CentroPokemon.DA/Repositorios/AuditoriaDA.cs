using CentroPokemon.BC.Entidades;
using CentroPokemon.BW.Interfaces.DA;
using CentroPokemon.DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CentroPokemon.DA.Repositorios;

public class AuditoriaDA : IAuditoriaDA
{
    private readonly CentroPokemonDbContext _context;

    public AuditoriaDA(CentroPokemonDbContext context)
    {
        _context = context;
    }

    public Task<List<Auditoria>> ListarAsync()
        => _context.Auditorias.Include(x => x.Usuario).OrderByDescending(x => x.FechaUtc).ToListAsync();

    public async Task CrearAsync(Auditoria auditoria)
    {
        _context.Auditorias.Add(auditoria);
        await _context.SaveChangesAsync();
    }
}
