using CentroPokemon.BC.DTOs.Citas;
using CentroPokemon.BW.Interfaces.BW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Get() => Ok(await _bw.ListarAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cita = await _bw.ObtenerPorIdAsync(id);
        return cita is null ? NotFound() : Ok(cita);
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
