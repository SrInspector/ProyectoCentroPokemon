using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.DA;

public interface ICitaDA
{
    Task<List<Cita>> ListarAsync(int? entrenadorId = null);
    Task<Cita?> ObtenerPorIdAsync(int id);
    Task CrearAsync(Cita cita);
    Task ActualizarAsync(Cita cita);
    Task EliminarAsync(Cita cita);
}
