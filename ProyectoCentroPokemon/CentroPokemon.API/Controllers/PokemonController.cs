using CentroPokemon.BC.DTOs.Pokemon;
using CentroPokemon.BC.DTOs.PokeApi;
using CentroPokemon.BW.Interfaces.BW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentroPokemon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PokemonController : ControllerBase
{
    private readonly IGestionarPokemonBW _bw;

    public PokemonController(IGestionarPokemonBW bw)
    {
        _bw = bw;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _bw.ListarAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var pokemon = await _bw.ObtenerPorIdAsync(id);
        return pokemon is null ? NotFound() : Ok(pokemon);
    }

    [HttpGet("pokeapi/{nombreOId}")]
    public async Task<IActionResult> GetDesdePokeApi(string nombreOId)
    {
        var pokemon = await _bw.BuscarEnPokeApiAsync(nombreOId);
        return pokemon is null ? NotFound() : Ok(pokemon);
    }

    [HttpPost("pokeapi/importar")]
    [Authorize(Roles = "Administrador,Enfermero")]
    public async Task<IActionResult> Importar([FromBody] PokemonImportRequest request)
    {
        try
        {
            var creado = await _bw.ImportarDesdePokeApiAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PokemonRequest request)
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
    public async Task<IActionResult> Put(int id, [FromBody] PokemonRequest request)
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
