using CentroPokemon.BC.DTOs.Citas;
using CentroPokemon.BW.Interfaces.BW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CentroPokemon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CitasController : ControllerBase
{
    private readonly IGestionarCitasBW _bw;

    public CitasController(IGestionarCitasBW bw)
    {
        _bw = bw;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        int? entrenadorIdFiltrado = null;
        if (User.IsInRole("Entrenador"))
        {
            var claim = User.FindFirst("entrenadorId");
            if (claim != null && int.TryParse(claim.Value, out var id))
            {
                entrenadorIdFiltrado = id;
            }
        }
        return Ok(await _bw.ListarAsync(entrenadorIdFiltrado));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cita = await _bw.ObtenerPorIdAsync(id);
        if (cita is null) return NotFound();

        if (User.IsInRole("Entrenador"))
        {
            var claim = User.FindFirst("entrenadorId");
            if (claim != null && int.TryParse(claim.Value, out var trainerId))
            {
                if (cita.EntrenadorId != trainerId) return Forbid();
            }
        }

        return Ok(cita);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CitaRequest request)
    {
        try
        {
            var creada = await _bw.CrearAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] CitaRequest request)
    {
        try
        {
            if (User.IsInRole("Entrenador"))
            {
                var claim = User.FindFirst("entrenadorId");
                if (claim != null && int.TryParse(claim.Value, out var trainerId))
                {
                    if (request.EntrenadorId != trainerId) return BadRequest(new { mensaje = "No puede asignar citas a otros entrenadores." });
                    
                    var citaExistente = await _bw.ObtenerPorIdAsync(id);
                    if (citaExistente != null && citaExistente.EntrenadorId != trainerId) return Forbid();
                }
            }

            var actualizada = await _bw.ActualizarAsync(id, request);
            return actualizada is null ? NotFound() : Ok(actualizada);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPatch("{id:int}/estado")]
    public async Task<IActionResult> PatchEstado(int id, [FromBody] ActualizarEstadoCitaRequest request)
    {
        var actualizada = await _bw.ActualizarEstadoAsync(id, request);
        return actualizada is null ? NotFound() : Ok(actualizada);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador,Recepcionista")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _bw.EliminarAsync(id) ? NoContent() : NotFound();
    }
}
