using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.DA;

public interface ICitaDA
{
    Task<List<Cita>> ListarAsync();
    Task<Cita?> ObtenerPorIdAsync(int id);
    Task CrearAsync(Cita cita);
    Task ActualizarAsync(Cita cita);
    Task EliminarAsync(Cita cita);
}
