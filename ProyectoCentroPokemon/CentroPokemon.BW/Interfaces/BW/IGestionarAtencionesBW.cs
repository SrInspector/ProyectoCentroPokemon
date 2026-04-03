using CentroPokemon.BC.DTOs.Atenciones;
using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.BW;

public interface IGestionarAtencionesBW
{
    Task<List<Atencion>> ListarAsync();
    Task<Atencion?> ObtenerPorIdAsync(int id);
    Task<Atencion> CrearAsync(int usuarioId, AtencionRequest request);
}
