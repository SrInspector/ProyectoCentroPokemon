using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.DA;

public interface IAtencionDA
{
    Task<List<Atencion>> ListarAsync();
    Task<Atencion?> ObtenerPorIdAsync(int id);
    Task CrearAsync(Atencion atencion);
}
