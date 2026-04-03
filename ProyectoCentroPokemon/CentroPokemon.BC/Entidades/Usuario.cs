using CentroPokemon.BC.Enumeradores;

namespace CentroPokemon.BC.Entidades;

public class Usuario
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public RolSistema Rol { get; set; }
    public int IntentosFallidos { get; set; }
    public DateTime? BloqueadoHastaUtc { get; set; }
    public DateTime? UltimoAccesoUtc { get; set; }
    public int? EntrenadorId { get; set; }
    public DateTime FechaCreacionUtc { get; set; } = DateTime.UtcNow;

    public Entrenador? Entrenador { get; set; }
    public ICollection<Cita> CitasAsignadas { get; set; } = new List<Cita>();
    public ICollection<Atencion> AtencionesRealizadas { get; set; } = new List<Atencion>();
    public ICollection<Internamiento> InternamientosRegistrados { get; set; } = new List<Internamiento>();
    public ICollection<TratamientoMedico> TratamientosRegistrados { get; set; } = new List<TratamientoMedico>();
    public ICollection<FacturaClinica> FacturasGeneradas { get; set; } = new List<FacturaClinica>();
    public ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
}
