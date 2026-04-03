using System.Security.Claims;
using CentroPokemon.BC.DTOs.Atenciones;
using CentroPokemon.BW.Interfaces.BW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentroPokemon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Enfermero")]
public class AtencionesController : ControllerBase
{
    private readonly IGestionarAtencionesBW _bw;

    public AtencionesController(IGestionarAtencionesBW bw)
    {
        _bw = bw;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _bw.ListarAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var atencion = await _bw.ObtenerPorIdAsync(id);
        return atencion is null ? NotFound() : Ok(atencion);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AtencionRequest request)
    {
        try
        {
            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
            {
                return Unauthorized();
            }

            var creada = await _bw.CrearAsync(usuarioId, request);
            return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
