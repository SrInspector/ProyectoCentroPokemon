using System.Security.Claims;
using CentroPokemon.BW.Interfaces.BW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentroPokemon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HistorialController : ControllerBase
{
    private readonly IGestionarHistorialBW _bw;

    public HistorialController(IGestionarHistorialBW bw)
    {
        _bw = bw;
    }

    [HttpGet("pokemon/{pokemonId:int}")]
    public async Task<IActionResult> GetHistorial(int pokemonId, [FromQuery] string? tipo, [FromQuery] string? estado, [FromQuery] DateTime? fechaInicioUtc, [FromQuery] DateTime? fechaFinUtc)
    {
        var historial = await _bw.ObtenerHistorialPokemonAsync(GetUsuarioId(), GetRol()!, pokemonId, tipo, estado, fechaInicioUtc, fechaFinUtc);
        return historial is null ? NotFound() : Ok(historial);
    }

    [HttpGet("pokemon/{pokemonId:int}/expediente")]
    public async Task<IActionResult> ExportarExpediente(int pokemonId, [FromQuery] string formato = "pdf")
    {
        try
        {
            var bytes = await _bw.ExportarExpedienteAsync(GetUsuarioId(), GetRol()!, pokemonId, formato);
            var contentType = formato.Equals("pdf", StringComparison.OrdinalIgnoreCase) ? "application/pdf" : "text/csv";
            var fileName = formato.Equals("pdf", StringComparison.OrdinalIgnoreCase) ? $"expediente-{pokemonId}.pdf" : $"expediente-{pokemonId}.csv";
            return File(bytes, contentType, fileName);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    private int GetUsuarioId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string? GetRol() => User.FindFirstValue(ClaimTypes.Role);
}
