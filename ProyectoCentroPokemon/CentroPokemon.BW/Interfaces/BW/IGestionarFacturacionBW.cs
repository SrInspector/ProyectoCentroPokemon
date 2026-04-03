using CentroPokemon.BC.DTOs.Facturacion;
using CentroPokemon.BC.Entidades;

namespace CentroPokemon.BW.Interfaces.BW;

public interface IGestionarFacturacionBW
{
    Task<List<ServicioClinico>> ListarServiciosAsync();
    Task<ServicioClinico> CrearServicioAsync(int usuarioId, ServicioClinicoRequest request);
    Task<List<FacturaClinica>> ListarFacturasAsync(int usuarioId, string rol);
    Task<FacturaClinica?> ObtenerFacturaAsync(int usuarioId, string rol, int id);
    Task<FacturaClinica> CrearFacturaAsync(int usuarioId, FacturaRequest request);
    Task<byte[]> GenerarComprobantePdfAsync(int usuarioId, string rol, int facturaId);
}
