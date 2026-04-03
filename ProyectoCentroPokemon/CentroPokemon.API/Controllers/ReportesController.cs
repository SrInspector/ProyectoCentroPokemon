using System.Security.Claims;
using CentroPokemon.BC.DTOs.Reportes;
using CentroPokemon.BW.Interfaces.BW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentroPokemon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class ReportesController : ControllerBase
{
    private readonly IGestionarReportesBW _bw;

    public ReportesController(IGestionarReportesBW bw)
    {
        _bw = bw;
    }

    [HttpPost]
    public async Task<IActionResult> Get([FromBody] FiltroFechasRequest filtro)
        => Ok(await _bw.ObtenerReportesAsync(GetUsuarioId(), filtro));

    [HttpPost("{tipo}/exportar")]
    public async Task<IActionResult> Exportar(string tipo, [FromBody] FiltroFechasRequest filtro)
    {
        var bytes = await _bw.ExportarReporteCsvAsync(GetUsuarioId(), tipo, filtro);
        return File(bytes, "text/csv", $"reporte-{tipo}.csv");
    }

    [HttpGet("auditoria")]
    public async Task<IActionResult> Auditoria()
        => Ok(await _bw.ListarAuditoriaAsync(GetUsuarioId()));

    private int GetUsuarioId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
