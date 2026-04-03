using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.DA;

public interface ITokenDA
{
    string GenerarToken(Usuario usuario);
}
