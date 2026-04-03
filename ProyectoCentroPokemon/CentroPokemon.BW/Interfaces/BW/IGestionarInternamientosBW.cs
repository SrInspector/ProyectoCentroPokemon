using CentroPokemon.BC.DTOs.Internamientos;
using CentroPokemon.BC.Entidades;
using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BW.Interfaces.BW;

public interface IGestionarInternamientosBW
{
    Task<List<Internamiento>> ListarAsync(int usuarioId, string rol, string? area, EstadoInternamiento? estado, DateTime? fechaInicioUtc, DateTime? fechaFinUtc);
    Task<Internamiento?> ObtenerPorIdAsync(int usuarioId, string rol, int id);
    Task<Internamiento> CrearAsync(int usuarioId, InternamientoRequest request);
    Task<Internamiento?> DarAltaMedicaAsync(int usuarioId, int id, AltaMedicaRequest request);
}
