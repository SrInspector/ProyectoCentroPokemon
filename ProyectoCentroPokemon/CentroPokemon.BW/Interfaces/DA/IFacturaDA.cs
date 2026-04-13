using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.DA;

public interface IFacturaDA
{
    Task<List<FacturaClinica>> ListarAsync(int? entrenadorId = null);
    Task<FacturaClinica?> ObtenerPorIdAsync(int id);
    Task CrearAsync(FacturaClinica factura);
    Task ActualizarAsync(FacturaClinica factura);
}
