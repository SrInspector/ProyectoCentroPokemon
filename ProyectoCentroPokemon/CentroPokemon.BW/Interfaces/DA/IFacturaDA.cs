using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.DA;

public interface IFacturaDA
{
    Task<List<FacturaClinica>> ListarAsync();
    Task<FacturaClinica?> ObtenerPorIdAsync(int id);
    Task<List<FacturaClinica>> ObtenerPorEntrenadorAsync(int entrenadorId);
    Task CrearAsync(FacturaClinica factura);
    Task ActualizarAsync(FacturaClinica factura);
}
