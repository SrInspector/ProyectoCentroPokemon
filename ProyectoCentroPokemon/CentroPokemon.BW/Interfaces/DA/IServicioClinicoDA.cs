using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.DA;

public interface IServicioClinicoDA
{
    Task<List<ServicioClinico>> ListarAsync();
    Task<ServicioClinico?> ObtenerPorIdAsync(int id);
    Task<ServicioClinico?> ObtenerPorCodigoAsync(string codigo);
    Task CrearAsync(ServicioClinico servicio);
    Task ActualizarAsync(ServicioClinico servicio);
}
