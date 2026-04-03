using CentroPokemon.BC.DTOs.Auth;
using CentroPokemon.BW.Interfaces.BW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentroPokemon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAutenticacionBW _autenticacionBW;

    public AuthController(IAutenticacionBW autenticacionBW)
    {
        _autenticacionBW = autenticacionBW;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            return Ok(await _autenticacionBW.LoginAsync(request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("usuarios")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioRequest request)
    {
        try
        {
            return Ok(await _autenticacionBW.CrearUsuarioAsync(request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
