using CentroPokemon.BC.DTOs.Tratamientos;
using CentroPokemon.BC.Entidades;
using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BW.Interfaces.BW;

public interface IGestionarTratamientosBW
{
    Task<List<TratamientoMedico>> ListarAsync(int usuarioId, string rol, string? tipo, EstadoTratamiento? estado, DateTime? fechaInicioUtc, DateTime? fechaFinUtc);
    Task<TratamientoMedico?> ObtenerPorIdAsync(int usuarioId, string rol, int id);
    Task<TratamientoMedico> CrearAsync(int usuarioId, TratamientoRequest request);
    Task<TratamientoMedico?> ActualizarAsync(int usuarioId, int id, TratamientoRequest request);
    Task<TratamientoMedico?> CambiarEstadoAsync(int usuarioId, int id, ActualizarEstadoTratamientoRequest request);
}
