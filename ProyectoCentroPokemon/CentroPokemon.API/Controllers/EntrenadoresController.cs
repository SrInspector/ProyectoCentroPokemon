using CentroPokemon.BC.DTOs.Entrenadores;
using CentroPokemon.BW.Interfaces.BW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentroPokemon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EntrenadoresController : ControllerBase
{
    private readonly IGestionarEntrenadoresBW _bw;

    public EntrenadoresController(IGestionarEntrenadoresBW bw)
    {
        _bw = bw;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _bw.ListarAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entrenador = await _bw.ObtenerPorIdAsync(id);
        return entrenador is null ? NotFound() : Ok(entrenador);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] EntrenadorRequest request)
    {
        try
        {
            var creado = await _bw.CrearAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] EntrenadorRequest request)
    {
        try
        {
            var actualizado = await _bw.ActualizarAsync(id, request);
            return actualizado is null ? NotFound() : Ok(actualizado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _bw.EliminarAsync(id) ? NoContent() : NotFound();
    }
}
