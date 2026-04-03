using System.Security.Claims;
using CentroPokemon.BC.DTOs.Facturacion;
using CentroPokemon.BW.Interfaces.BW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentroPokemon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FacturacionController : ControllerBase
{
    private readonly IGestionarFacturacionBW _bw;

    public FacturacionController(IGestionarFacturacionBW bw)
    {
        _bw = bw;
    }

    [HttpGet("servicios")]
    public async Task<IActionResult> GetServicios() => Ok(await _bw.ListarServiciosAsync());

    [HttpPost("servicios")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> PostServicio([FromBody] ServicioClinicoRequest request)
    {
        try
        {
            return Ok(await _bw.CrearServicioAsync(GetUsuarioId(), request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("facturas")]
    public async Task<IActionResult> GetFacturas() => Ok(await _bw.ListarFacturasAsync(GetUsuarioId(), GetRol()!));

    [HttpGet("facturas/{id:int}")]
    public async Task<IActionResult> GetFactura(int id)
    {
        var factura = await _bw.ObtenerFacturaAsync(GetUsuarioId(), GetRol()!, id);
        return factura is null ? NotFound() : Ok(factura);
    }

    [HttpPost("facturas")]
    [Authorize(Roles = "Administrador,Enfermero")]
    public async Task<IActionResult> PostFactura([FromBody] FacturaRequest request)
    {
        try
        {
            var factura = await _bw.CrearFacturaAsync(GetUsuarioId(), request);
            return CreatedAtAction(nameof(GetFactura), new { id = factura.Id }, factura);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("facturas/{id:int}/comprobante")]
    public async Task<IActionResult> DescargarComprobante(int id)
    {
        try
        {
            var pdf = await _bw.GenerarComprobantePdfAsync(GetUsuarioId(), GetRol()!, id);
            return File(pdf, "application/pdf", $"comprobante-{id}.pdf");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    private int GetUsuarioId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string? GetRol() => User.FindFirstValue(ClaimTypes.Role);
}
