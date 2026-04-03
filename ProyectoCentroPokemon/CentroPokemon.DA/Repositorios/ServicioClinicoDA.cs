using CentroPokemon.BC.Entidades;
using CentroPokemon.BW.Interfaces.DA;
using CentroPokemon.DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CentroPokemon.DA.Repositorios;

public class ServicioClinicoDA : IServicioClinicoDA
{
    private readonly CentroPokemonDbContext _context;

    public ServicioClinicoDA(CentroPokemonDbContext context)
    {
        _context = context;
    }

    public Task<List<ServicioClinico>> ListarAsync()
        => _context.ServiciosClinicos.OrderBy(x => x.Nombre).ToListAsync();

    public Task<ServicioClinico?> ObtenerPorIdAsync(int id)
        => _context.ServiciosClinicos.FirstOrDefaultAsync(x => x.Id == id);

    public Task<ServicioClinico?> ObtenerPorCodigoAsync(string codigo)
        => _context.ServiciosClinicos.FirstOrDefaultAsync(x => x.Codigo == codigo);

    public async Task CrearAsync(ServicioClinico servicio)
    {
        _context.ServiciosClinicos.Add(servicio);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(ServicioClinico servicio)
    {
        _context.ServiciosClinicos.Update(servicio);
        await _context.SaveChangesAsync();
    }
}
