using System.Text;
using CentroPokemon.BC.DTOs.Facturacion;
using CentroPokemon.BC.Entidades;
using CentroPokemon.BC.Enumeradores;
using CentroPokemon.BW.Interfaces.BW;
using CentroPokemon.BW.Interfaces.DA;

namespace CentroPokemon.BW.CU;

public class GestionarFacturacionBW : IGestionarFacturacionBW
{
    private readonly IServicioClinicoDA _servicioDA;
    private readonly IFacturaDA _facturaDA;
    private readonly IEntrenadorDA _entrenadorDA;
    private readonly IUsuarioDA _usuarioDA;
    private readonly IAuditoriaBW _auditoriaBW;

    public GestionarFacturacionBW(IServicioClinicoDA servicioDA, IFacturaDA facturaDA, IEntrenadorDA entrenadorDA, IUsuarioDA usuarioDA, IAuditoriaBW auditoriaBW)
    {
        _servicioDA = servicioDA;
        _facturaDA = facturaDA;
        _entrenadorDA = entrenadorDA;
        _usuarioDA = usuarioDA;
        _auditoriaBW = auditoriaBW;
    }

    public Task<List<ServicioClinico>> ListarServiciosAsync() => _servicioDA.ListarAsync();

    public async Task<ServicioClinico> CrearServicioAsync(int usuarioId, ServicioClinicoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Codigo) || string.IsNullOrWhiteSpace(request.Nombre))
        {
            throw new InvalidOperationException("El codigo y nombre del servicio son obligatorios.");
        }

        if (await _servicioDA.ObtenerPorCodigoAsync(request.Codigo.Trim().ToUpperInvariant()) is not null)
        {
            throw new InvalidOperationException("Ya existe un servicio con ese codigo.");
        }

        var servicio = new ServicioClinico
        {
            Codigo = request.Codigo.Trim().ToUpperInvariant(),
            Nombre = request.Nombre.Trim(),
            Descripcion = request.Descripcion.Trim(),
            CostoBase = request.CostoBase,
            RequiereAprobacionAdministrador = request.RequiereAprobacionAdministrador,
            EsRecurrente = request.EsRecurrente
        };

        await _servicioDA.CrearAsync(servicio);
        await _auditoriaBW.RegistrarAsync(usuarioId, "ServicioClinico", servicio.Codigo, TipoOperacionAuditoria.Crear, "Servicio clinico registrado.");
        return servicio;
    }

    public async Task<List<FacturaClinica>> ListarFacturasAsync(int usuarioId, string rol)
    {
        if (rol == RolSistema.Entrenador.ToString())
        {
            var usuario = await _usuarioDA.ObtenerPorIdAsync(usuarioId)
                ?? throw new InvalidOperationException("El usuario no existe.");

            if (!usuario.EntrenadorId.HasValue)
            {
                return new List<FacturaClinica>();
            }

            return await _facturaDA.ObtenerPorEntrenadorAsync(usuario.EntrenadorId.Value);
        }

        return await _facturaDA.ListarAsync();
    }

    public async Task<FacturaClinica?> ObtenerFacturaAsync(int usuarioId, string rol, int id)
    {
        var factura = await _facturaDA.ObtenerPorIdAsync(id);
        if (factura is null)
        {
            return null;
        }

        if (rol == RolSistema.Entrenador.ToString())
        {
            var usuario = await _usuarioDA.ObtenerPorIdAsync(usuarioId);
            if (usuario?.EntrenadorId != factura.EntrenadorId)
            {
                return null;
            }
        }

        return factura;
    }

    public async Task<FacturaClinica> CrearFacturaAsync(int usuarioId, FacturaRequest request)
    {
        if (await _entrenadorDA.ObtenerPorIdAsync(request.EntrenadorId) is null)
        {
            throw new InvalidOperationException("El entrenador indicado no existe.");
        }

        if (!request.Detalles.Any())
        {
            throw new InvalidOperationException("Debe incluir al menos un detalle de factura.");
        }

        var factura = new FacturaClinica
        {
            Referencia = $"FAC-{DateTime.UtcNow:yyyyMMddHHmmss}-{request.EntrenadorId}",
            EntrenadorId = request.EntrenadorId,
            UsuarioGeneradorId = usuarioId,
            Estado = EstadoFactura.Pendiente
        };

        foreach (var item in request.Detalles)
        {
            var servicio = await _servicioDA.ObtenerPorIdAsync(item.ServicioClinicoId)
                ?? throw new InvalidOperationException($"No existe el servicio {item.ServicioClinicoId}.");

            factura.Detalles.Add(new FacturaDetalle
            {
                ServicioClinicoId = servicio.Id,
                Cantidad = item.Cantidad,
                PrecioUnitario = servicio.CostoBase,
                Subtotal = servicio.CostoBase * item.Cantidad
            });
        }

        factura.Total = factura.Detalles.Sum(x => x.Subtotal);
        await _facturaDA.CrearAsync(factura);
        await _auditoriaBW.RegistrarAsync(usuarioId, "FacturaClinica", factura.Referencia, TipoOperacionAuditoria.Crear, "Factura clinica generada.");
        return factura;
    }

    public async Task<byte[]> GenerarComprobantePdfAsync(int usuarioId, string rol, int facturaId)
    {
        var factura = await ObtenerFacturaAsync(usuarioId, rol, facturaId)
            ?? throw new InvalidOperationException("La factura indicada no existe o no esta disponible.");

        var lines = new List<string>
        {
            "Centro Pokemon",
            $"Referencia: {factura.Referencia}",
            $"Fecha: {factura.FechaEmisionUtc:yyyy-MM-dd HH:mm}",
            $"Entrenador: {factura.Entrenador?.Nombre}",
            $"Total: {factura.Total:C}",
            "Detalles:"
        };

        lines.AddRange(factura.Detalles.Select(d => $"{d.ServicioClinico?.Nombre} x{d.Cantidad} = {d.Subtotal:C}"));

        var content = string.Join("\n", lines);
        await _auditoriaBW.RegistrarAsync(usuarioId, "FacturaClinica", factura.Referencia, TipoOperacionAuditoria.Descargar, "Comprobante PDF descargado.");
        return SimplePdfGenerator.Generate("Comprobante Clinico", content);
    }
}
