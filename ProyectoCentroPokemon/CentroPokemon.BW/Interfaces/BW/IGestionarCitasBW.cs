using CentroPokemon.BC.DTOs.Citas;
using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.BW;

public interface IGestionarCitasBW
{
    Task<List<Cita>> ListarAsync(int? entrenadorId = null);
    Task<Cita?> ObtenerPorIdAsync(int id);
    Task<Cita> CrearAsync(CitaRequest request);
    Task<Cita?> ActualizarAsync(int id, CitaRequest request);
    Task<Cita?> ActualizarEstadoAsync(int id, ActualizarEstadoCitaRequest request);
    Task<bool> EliminarAsync(int id);
}
