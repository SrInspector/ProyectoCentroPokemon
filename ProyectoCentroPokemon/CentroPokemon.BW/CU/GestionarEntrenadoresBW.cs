using CentroPokemon.BC.DTOs.Entrenadores;
using CentroPokemon.BC.Entidades;
using CentroPokemon.BW.Interfaces.BW;
using CentroPokemon.BW.Interfaces.DA;

namespace CentroPokemon.BW.CU;

public class GestionarEntrenadoresBW : IGestionarEntrenadoresBW
{
    private readonly IEntrenadorDA _entrenadorDA;
    private readonly IAuditoriaBW _auditoriaBW;

    public GestionarEntrenadoresBW(IEntrenadorDA entrenadorDA, IAuditoriaBW auditoriaBW)
    {
        _entrenadorDA = entrenadorDA;
        _auditoriaBW = auditoriaBW;
    }

    public Task<List<Entrenador>> ListarAsync() => _entrenadorDA.ListarAsync();

    public Task<Entrenador?> ObtenerPorIdAsync(int id) => _entrenadorDA.ObtenerPorIdAsync(id);

    public async Task<Entrenador> CrearAsync(EntrenadorRequest request)
    {
        Validar(request);
        var entrenador = new Entrenador
        {
            Identificacion = request.Identificacion.Trim(),
            Nombre = request.Nombre.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Telefono = request.Telefono.Trim()
        };

        await _entrenadorDA.CrearAsync(entrenador);
        await _auditoriaBW.RegistrarAsync(null, "Entrenador", entrenador.Id.ToString(), BC.Enumeradores.TipoOperacionAuditoria.Crear, "Entrenador registrado.");
        return entrenador;
    }

    public async Task<Entrenador?> ActualizarAsync(int id, EntrenadorRequest request)
    {
        Validar(request);
        var entrenador = await _entrenadorDA.ObtenerPorIdAsync(id);
        if (entrenador is null)
        {
            return null;
        }

        entrenador.Identificacion = request.Identificacion.Trim();
        entrenador.Nombre = request.Nombre.Trim();
        entrenador.Email = request.Email.Trim().ToLowerInvariant();
        entrenador.Telefono = request.Telefono.Trim();

        await _entrenadorDA.ActualizarAsync(entrenador);
        await _auditoriaBW.RegistrarAsync(null, "Entrenador", entrenador.Id.ToString(), BC.Enumeradores.TipoOperacionAuditoria.Actualizar, "Entrenador actualizado.");
        return entrenador;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var entrenador = await _entrenadorDA.ObtenerPorIdAsync(id);
        if (entrenador is null)
        {
            return false;
        }

        await _entrenadorDA.EliminarAsync(entrenador);
        await _auditoriaBW.RegistrarAsync(null, "Entrenador", entrenador.Id.ToString(), BC.Enumeradores.TipoOperacionAuditoria.Eliminar, "Entrenador eliminado.");
        return true;
    }

    private static void Validar(EntrenadorRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            throw new InvalidOperationException("El nombre del entrenador es requerido.");
        }

        if (string.IsNullOrWhiteSpace(request.Identificacion))
        {
            throw new InvalidOperationException("La identificacion del entrenador es requerida.");
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
        {
            throw new InvalidOperationException("El correo del entrenador no es valido.");
        }
    }
}
