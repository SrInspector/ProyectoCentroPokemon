using CentroPokemon.BC.Entidades;
using CentroPokemon.BC.Enumeradores;
using CentroPokemon.BW.Interfaces.BW;
using CentroPokemon.BW.Interfaces.DA;

namespace CentroPokemon.BW.CU;

public class AuditoriaBW : IAuditoriaBW
{
    private readonly IAuditoriaDA _auditoriaDA;

    public AuditoriaBW(IAuditoriaDA auditoriaDA)
    {
        _auditoriaDA = auditoriaDA;
    }

    public Task RegistrarAsync(int? usuarioId, string entidad, string entidadId, TipoOperacionAuditoria operacion, string descripcion)
    {
        return _auditoriaDA.CrearAsync(new Auditoria
        {
            UsuarioId = usuarioId,
            Entidad = entidad,
            EntidadId = entidadId,
            Operacion = operacion,
            Descripcion = descripcion
        });
    }
}
