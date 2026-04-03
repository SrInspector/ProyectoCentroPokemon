using CentroPokemon.BC.DTOs.Atenciones;
using CentroPokemon.BC.Entidades;
using CentroPokemon.BC.Enumeradores;
using CentroPokemon.BW.Interfaces.BW;
using CentroPokemon.BW.Interfaces.DA;

namespace CentroPokemon.BW.CU;

public class GestionarAtencionesBW : IGestionarAtencionesBW
{
    private readonly IAtencionDA _atencionDA;
    private readonly ICitaDA _citaDA;
    private readonly IPokemonDA _pokemonDA;
    private readonly IUsuarioDA _usuarioDA;

    public GestionarAtencionesBW(IAtencionDA atencionDA, ICitaDA citaDA, IPokemonDA pokemonDA, IUsuarioDA usuarioDA)
    {
        _atencionDA = atencionDA;
        _citaDA = citaDA;
        _pokemonDA = pokemonDA;
        _usuarioDA = usuarioDA;
    }

    public Task<List<Atencion>> ListarAsync() => _atencionDA.ListarAsync();

    public Task<Atencion?> ObtenerPorIdAsync(int id) => _atencionDA.ObtenerPorIdAsync(id);

    public async Task<Atencion> CrearAsync(int usuarioId, AtencionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Diagnostico) || string.IsNullOrWhiteSpace(request.Tratamiento))
        {
            throw new InvalidOperationException("El diagnostico y el tratamiento son requeridos.");
        }

        if (await _usuarioDA.ObtenerPorIdAsync(usuarioId) is null)
        {
            throw new InvalidOperationException("El usuario autenticado no existe.");
        }

        var cita = await _citaDA.ObtenerPorIdAsync(request.CitaId)
            ?? throw new InvalidOperationException("La cita indicada no existe.");

        if (cita.Atencion is not null)
        {
            throw new InvalidOperationException("La cita ya tiene una atencion registrada.");
        }

        var pokemon = await _pokemonDA.ObtenerPorIdAsync(cita.PokemonId)
            ?? throw new InvalidOperationException("El pokemon asociado a la cita no existe.");

        var atencion = new Atencion
        {
            CitaId = cita.Id,
            PokemonId = cita.PokemonId,
            UsuarioId = usuarioId,
            Diagnostico = request.Diagnostico.Trim(),
            Tratamiento = request.Tratamiento.Trim(),
            Observaciones = string.IsNullOrWhiteSpace(request.Observaciones) ? null : request.Observaciones.Trim()
        };

        cita.Estado = EstadoCita.Atendida;
        pokemon.EstadoSalud = EstadoPokemon.EnObservacion;

        await _atencionDA.CrearAsync(atencion);
        await _citaDA.ActualizarAsync(cita);
        await _pokemonDA.ActualizarAsync(pokemon);

        return atencion;
    }
}
