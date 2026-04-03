using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.DA;

public interface IAuditoriaDA
{
    Task<List<Auditoria>> ListarAsync();
    Task CrearAsync(Auditoria auditoria);
}
