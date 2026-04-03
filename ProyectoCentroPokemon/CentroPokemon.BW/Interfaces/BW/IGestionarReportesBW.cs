using CentroPokemon.BC.DTOs.Reportes;
using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.BW;

public interface IGestionarReportesBW
{
    Task<object> ObtenerReportesAsync(int usuarioId, FiltroFechasRequest filtro);
    Task<byte[]> ExportarReporteCsvAsync(int usuarioId, string tipo, FiltroFechasRequest filtro);
    Task<List<Auditoria>> ListarAuditoriaAsync(int usuarioId);
}
