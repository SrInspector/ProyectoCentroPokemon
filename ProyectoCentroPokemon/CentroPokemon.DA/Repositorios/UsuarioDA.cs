using CentroPokemon.BC.Entidades;
using CentroPokemon.BW.Interfaces.DA;
using CentroPokemon.DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CentroPokemon.DA.Repositorios;

public class UsuarioDA : IUsuarioDA
{
    private readonly CentroPokemonDbContext _context;

    public UsuarioDA(CentroPokemonDbContext context)
    {
        _context = context;
    }

    public Task<Usuario?> ObtenerPorCorreoAsync(string correo)
        => _context.Usuarios.Include(x => x.Entrenador).FirstOrDefaultAsync(x => x.Correo == correo);

    public Task<Usuario?> ObtenerPorIdAsync(int id)
        => _context.Usuarios.Include(x => x.Entrenador).FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<Usuario>> ListarAsync()
        => _context.Usuarios.Include(x => x.Entrenador).OrderBy(x => x.NombreCompleto).ToListAsync();

    public async Task CrearAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }
}
