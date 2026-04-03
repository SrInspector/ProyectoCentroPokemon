using System.Text;
using CentroPokemon.BC.DTOs.Reportes;
using CentroPokemon.BW.Interfaces.BW;
using CentroPokemon.BW.Interfaces.DA;

namespace CentroPokemon.BW.CU;

public class GestionarReportesBW : IGestionarReportesBW
{
    private readonly IReporteDA _reporteDA;
    private readonly IAuditoriaDA _auditoriaDA;
    private readonly IAuditoriaBW _auditoriaBW;

    public GestionarReportesBW(IReporteDA reporteDA, IAuditoriaDA auditoriaDA, IAuditoriaBW auditoriaBW)
    {
        _reporteDA = reporteDA;
        _auditoriaDA = auditoriaDA;
        _auditoriaBW = auditoriaBW;
    }

    public async Task<object> ObtenerReportesAsync(int usuarioId, FiltroFechasRequest filtro)
    {
        var atenciones = await _reporteDA.ObtenerAtencionesPorPeriodoAsync(filtro.FechaInicioUtc, filtro.FechaFinUtc);
        var topPokemon = await _reporteDA.ObtenerTopPokemonPorTratamientosAsync(filtro.FechaInicioUtc, filtro.FechaFinUtc);
        var usoAreas = await _reporteDA.ObtenerUsoAreasClinicasAsync(filtro.FechaInicioUtc, filtro.FechaFinUtc);

        return new
        {
            AtencionesPorPeriodo = atenciones,
            TopPokemonPorTratamientos = topPokemon,
            UsoAreasClinicas = usoAreas
        };
    }

    public async Task<byte[]> ExportarReporteCsvAsync(int usuarioId, string tipo, FiltroFechasRequest filtro)
    {
        var sb = new StringBuilder();
        if (tipo.Equals("atenciones", StringComparison.OrdinalIgnoreCase))
        {
            var data = await _reporteDA.ObtenerAtencionesPorPeriodoAsync(filtro.FechaInicioUtc, filtro.FechaFinUtc);
            sb.AppendLine("Fecha,Cantidad");
            foreach (var item in data) sb.AppendLine($"{item.Fecha:yyyy-MM-dd},{item.Cantidad}");
        }
        else if (tipo.Equals("top-pokemon", StringComparison.OrdinalIgnoreCase))
        {
            var data = await _reporteDA.ObtenerTopPokemonPorTratamientosAsync(filtro.FechaInicioUtc, filtro.FechaFinUtc);
            sb.AppendLine("Pokemon,CantidadTratamientos");
            foreach (var item in data) sb.AppendLine($"{item.Pokemon},{item.CantidadTratamientos}");
        }
        else
        {
            var data = await _reporteDA.ObtenerUsoAreasClinicasAsync(filtro.FechaInicioUtc, filtro.FechaFinUtc);
            sb.AppendLine("Area,CantidadInternamientos");
            foreach (var item in data) sb.AppendLine($"{item.AreaAsignada},{item.CantidadInternamientos}");
        }

        await _auditoriaBW.RegistrarAsync(usuarioId, "Reporte", tipo, BC.Enumeradores.TipoOperacionAuditoria.Exportar, "Reporte exportado a CSV.");
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public Task<List<BC.Entidades.Auditoria>> ListarAuditoriaAsync(int usuarioId) => _auditoriaDA.ListarAsync();
}
