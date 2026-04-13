using CentroPokemon.BC.DTOs.Pokemon;
using CentroPokemon.BC.DTOs.PokeApi;
using CentroPokemon.BC.Entidades;
using CentroPokemon.BW.Interfaces.BW;
using CentroPokemon.BW.Interfaces.DA;

namespace CentroPokemon.BW.CU;

public class GestionarPokemonBW : IGestionarPokemonBW
{
    private readonly IPokemonDA _pokemonDA;
    private readonly IEntrenadorDA _entrenadorDA;
    private readonly IAuditoriaBW _auditoriaBW;
    private readonly IPokeApiDA _pokeApiDA;

    public GestionarPokemonBW(IPokemonDA pokemonDA, IEntrenadorDA entrenadorDA, IAuditoriaBW auditoriaBW, IPokeApiDA pokeApiDA)
    {
        _pokemonDA = pokemonDA;
        _entrenadorDA = entrenadorDA;
        _auditoriaBW = auditoriaBW;
        _pokeApiDA = pokeApiDA;
    }

    public Task<List<Pokemon>> ListarAsync(int? entrenadorId = null) => _pokemonDA.ListarAsync(entrenadorId);

    public Task<Pokemon?> ObtenerPorIdAsync(int id) => _pokemonDA.ObtenerPorIdAsync(id);

    public Task<PokeApiPokemonDto?> BuscarEnPokeApiAsync(string nombreOId) => _pokeApiDA.ObtenerPokemonAsync(nombreOId);

    public async Task<Pokemon> ImportarDesdePokeApiAsync(PokemonImportRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NombreOId))
        {
            throw new InvalidOperationException("Debe indicar el nombre o id del pokemon en PokeAPI.");
        }

        var pokeApiPokemon = await _pokeApiDA.ObtenerPokemonAsync(request.NombreOId)
            ?? throw new InvalidOperationException("No se encontro el pokemon solicitado en PokeAPI.");

        if (await _entrenadorDA.ObtenerPorIdAsync(request.EntrenadorId) is null)
        {
            throw new InvalidOperationException("El entrenador indicado no existe.");
        }

        var identificador = request.IdentificadorUnico.Trim().ToUpperInvariant();
        if (await _pokemonDA.ObtenerPorIdentificadorUnicoAsync(identificador) is not null)
        {
            throw new InvalidOperationException("Ya existe un pokemon local con ese identificador unico.");
        }

        var tipos = pokeApiPokemon.Types.OrderBy(x => x.Slot).Select(x => x.Type?.Name).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        var pokemon = new Pokemon
        {
            IdentificadorUnico = identificador,
            Nombre = string.IsNullOrWhiteSpace(request.NombreAsignado) ? Capitalizar(pokeApiPokemon.Name) : request.NombreAsignado.Trim(),
            Especie = Capitalizar(pokeApiPokemon.Name),
            Nivel = request.Nivel,
            TipoPrimario = Capitalizar(tipos.ElementAtOrDefault(0) ?? "Normal"),
            TipoSecundario = tipos.Count > 1 ? Capitalizar(tipos[1]!) : null,
            EstadoSalud = BC.Enumeradores.EstadoPokemon.Estable,
            EntrenadorId = request.EntrenadorId
        };

        await _pokemonDA.CrearAsync(pokemon);
        await _auditoriaBW.RegistrarAsync(null, "Pokemon", pokemon.Id.ToString(), BC.Enumeradores.TipoOperacionAuditoria.Crear, $"Pokemon importado desde PokeAPI: {pokeApiPokemon.Name}.");
        return pokemon;
    }

    public async Task<Pokemon> CrearAsync(PokemonRequest request)
    {
        await ValidarAsync(request);
        var pokemon = new Pokemon
        {
            IdentificadorUnico = request.IdentificadorUnico.Trim().ToUpperInvariant(),
            Nombre = request.Nombre.Trim(),
            Especie = request.Especie.Trim(),
            Nivel = request.Nivel,
            TipoPrimario = request.TipoPrimario.Trim(),
            TipoSecundario = string.IsNullOrWhiteSpace(request.TipoSecundario) ? null : request.TipoSecundario.Trim(),
            EstadoSalud = request.EstadoSalud,
            EntrenadorId = request.EntrenadorId
        };

        await _pokemonDA.CrearAsync(pokemon);
        await _auditoriaBW.RegistrarAsync(null, "Pokemon", pokemon.Id.ToString(), BC.Enumeradores.TipoOperacionAuditoria.Crear, "Pokemon registrado.");
        return pokemon;
    }

    public async Task<Pokemon?> ActualizarAsync(int id, PokemonRequest request)
    {
        await ValidarAsync(request);
        var pokemon = await _pokemonDA.ObtenerPorIdAsync(id);
        if (pokemon is null)
        {
            return null;
        }

        pokemon.IdentificadorUnico = request.IdentificadorUnico.Trim().ToUpperInvariant();
        pokemon.Nombre = request.Nombre.Trim();
        pokemon.Especie = request.Especie.Trim();
        pokemon.Nivel = request.Nivel;
        pokemon.TipoPrimario = request.TipoPrimario.Trim();
        pokemon.TipoSecundario = string.IsNullOrWhiteSpace(request.TipoSecundario) ? null : request.TipoSecundario.Trim();
        pokemon.EstadoSalud = request.EstadoSalud;
        pokemon.EntrenadorId = request.EntrenadorId;

        await _pokemonDA.ActualizarAsync(pokemon);
        await _auditoriaBW.RegistrarAsync(null, "Pokemon", pokemon.Id.ToString(), BC.Enumeradores.TipoOperacionAuditoria.Actualizar, "Pokemon actualizado.");
        return pokemon;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var pokemon = await _pokemonDA.ObtenerPorIdAsync(id);
        if (pokemon is null)
        {
            return false;
        }

        await _pokemonDA.EliminarAsync(pokemon);
        await _auditoriaBW.RegistrarAsync(null, "Pokemon", pokemon.Id.ToString(), BC.Enumeradores.TipoOperacionAuditoria.Eliminar, "Pokemon eliminado.");
        return true;
    }

    private async Task ValidarAsync(PokemonRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.IdentificadorUnico) || string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Especie))
        {
            throw new InvalidOperationException("El identificador unico, nombre y especie del pokemon son requeridos.");
        }

        if (request.Nivel < 1 || request.Nivel > 100)
        {
            throw new InvalidOperationException("El nivel del pokemon debe estar entre 1 y 100.");
        }

        if (await _entrenadorDA.ObtenerPorIdAsync(request.EntrenadorId) is null)
        {
            throw new InvalidOperationException("El entrenador indicado no existe.");
        }

        var existente = await _pokemonDA.ObtenerPorIdentificadorUnicoAsync(request.IdentificadorUnico.Trim().ToUpperInvariant());
        if (existente is not null)
        {
            throw new InvalidOperationException("Ya existe un pokemon con ese identificador unico.");
        }
    }

    private static string Capitalizar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return valor;
        }

        return char.ToUpperInvariant(valor[0]) + valor[1..].ToLowerInvariant();
    }
}
