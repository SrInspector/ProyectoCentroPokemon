using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BW.Interfaces.BW;

public interface IAuditoriaBW
{
    Task RegistrarAsync(int? usuarioId, string entidad, string entidadId, TipoOperacionAuditoria operacion, string descripcion);
}
