using CentroPokemon.BC.DTOs.Inventario;
using CentroPokemon.BW.Interfaces.BW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentroPokemon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventarioController : ControllerBase
{
    private readonly IGestionarInventarioBW _bw;

    public InventarioController(IGestionarInventarioBW bw)
    {
        _bw = bw;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _bw.ListarAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _bw.ObtenerPorIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Recepcionista")]
    public async Task<IActionResult> Post([FromBody] ItemInventarioRequest request)
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
    [Authorize(Roles = "Administrador,Recepcionista")]
    public async Task<IActionResult> Put(int id, [FromBody] ItemInventarioRequest request)
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
