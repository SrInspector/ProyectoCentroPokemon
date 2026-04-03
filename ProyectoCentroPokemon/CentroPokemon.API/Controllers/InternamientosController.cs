using System.Security.Claims;
using CentroPokemon.BC.DTOs.Internamientos;
using CentroPokemon.BC.Enumeradores;
using CentroPokemon.BW.Interfaces.BW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentroPokemon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InternamientosController : ControllerBase
{
    private readonly IGestionarInternamientosBW _bw;

    public InternamientosController(IGestionarInternamientosBW bw)
    {
        _bw = bw;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? area, [FromQuery] EstadoInternamiento? estado, [FromQuery] DateTime? fechaInicioUtc, [FromQuery] DateTime? fechaFinUtc)
    {
        return Ok(await _bw.ListarAsync(GetUsuarioId(), GetRol()!, area, estado, fechaInicioUtc, fechaFinUtc));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _bw.ObtenerPorIdAsync(GetUsuarioId(), GetRol()!, id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Enfermero")]
    public async Task<IActionResult> Post([FromBody] InternamientoRequest request)
    {
        try
        {
            var creado = await _bw.CrearAsync(GetUsuarioId(), request);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPatch("{id:int}/alta")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> AltaMedica(int id, [FromBody] AltaMedicaRequest request)
    {
        try
        {
            var actualizado = await _bw.DarAltaMedicaAsync(GetUsuarioId(), id, request);
            return actualizado is null ? NotFound() : Ok(actualizado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    private int GetUsuarioId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string? GetRol() => User.FindFirstValue(ClaimTypes.Role);
}
