using CentroPokemon.BC.DTOs.Reportes;

namespace CentroPokemon.BW.Interfaces.DA;

public interface IReporteDA
{
    Task<List<ReporteAtencionesItem>> ObtenerAtencionesPorPeriodoAsync(DateTime? fechaInicioUtc, DateTime? fechaFinUtc);
    Task<List<ReporteTopPokemonItem>> ObtenerTopPokemonPorTratamientosAsync(DateTime? fechaInicioUtc, DateTime? fechaFinUtc);
    Task<List<ReporteUsoAreaItem>> ObtenerUsoAreasClinicasAsync(DateTime? fechaInicioUtc, DateTime? fechaFinUtc);
}
