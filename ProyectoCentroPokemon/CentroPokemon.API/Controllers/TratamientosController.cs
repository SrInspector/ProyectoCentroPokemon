using System.Security.Claims;
using CentroPokemon.BC.DTOs.Tratamientos;
using CentroPokemon.BC.Enumeradores;
using CentroPokemon.BW.Interfaces.BW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentroPokemon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TratamientosController : ControllerBase
{
    private readonly IGestionarTratamientosBW _bw;

    public TratamientosController(IGestionarTratamientosBW bw)
    {
        _bw = bw;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? tipo, [FromQuery] EstadoTratamiento? estado, [FromQuery] DateTime? fechaInicioUtc, [FromQuery] DateTime? fechaFinUtc)
    {
        return Ok(await _bw.ListarAsync(GetUsuarioId(), GetRol()!, tipo, estado, fechaInicioUtc, fechaFinUtc));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _bw.ObtenerPorIdAsync(GetUsuarioId(), GetRol()!, id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Enfermero")]
    public async Task<IActionResult> Post([FromBody] TratamientoRequest request)
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

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador,Enfermero")]
    public async Task<IActionResult> Put(int id, [FromBody] TratamientoRequest request)
    {
        try
        {
            var actualizado = await _bw.ActualizarAsync(GetUsuarioId(), id, request);
            return actualizado is null ? NotFound() : Ok(actualizado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = "Administrador,Enfermero")]
    public async Task<IActionResult> PatchEstado(int id, [FromBody] ActualizarEstadoTratamientoRequest request)
    {
        try
        {
            var actualizado = await _bw.CambiarEstadoAsync(GetUsuarioId(), id, request);
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
