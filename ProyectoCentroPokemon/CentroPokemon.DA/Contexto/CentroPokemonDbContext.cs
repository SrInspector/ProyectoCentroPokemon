using CentroPokemon.BC.Entidades;
using Microsoft.EntityFrameworkCore;

namespace CentroPokemon.DA.Contexto;

public class CentroPokemonDbContext : DbContext
{
    public CentroPokemonDbContext(DbContextOptions<CentroPokemonDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Entrenador> Entrenadores => Set<Entrenador>();
    public DbSet<Pokemon> Pokemones => Set<Pokemon>();
    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<Atencion> Atenciones => Set<Atencion>();
    public DbSet<ItemInventario> Inventario => Set<ItemInventario>();
    public DbSet<Internamiento> Internamientos => Set<Internamiento>();
    public DbSet<TratamientoMedico> TratamientosMedicos => Set<TratamientoMedico>();
    public DbSet<ServicioClinico> ServiciosClinicos => Set<ServicioClinico>();
    public DbSet<FacturaClinica> FacturasClinicas => Set<FacturaClinica>();
    public DbSet<FacturaDetalle> FacturasDetalle => Set<FacturaDetalle>();
    public DbSet<Auditoria> Auditorias => Set<Auditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.NombreCompleto).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Correo).HasMaxLength(150).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Rol).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.HasIndex(x => x.Correo).IsUnique();
            entity.HasOne(x => x.Entrenador)
                .WithMany(x => x.Usuarios)
                .HasForeignKey(x => x.EntrenadorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Entrenador>(entity =>
        {
            entity.ToTable("Entrenadores");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Identificacion).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Telefono).HasMaxLength(30).IsRequired();
            entity.HasIndex(x => x.Identificacion).IsUnique();
        });

        modelBuilder.Entity<Pokemon>(entity =>
        {
            entity.ToTable("Pokemones");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.IdentificadorUnico).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Especie).HasMaxLength(120).IsRequired();
            entity.Property(x => x.TipoPrimario).HasMaxLength(40).IsRequired();
            entity.Property(x => x.TipoSecundario).HasMaxLength(40);
            entity.Property(x => x.EstadoSalud).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.HasIndex(x => x.IdentificadorUnico).IsUnique();
            entity.HasOne(x => x.Entrenador)
                .WithMany(x => x.Pokemones)
                .HasForeignKey(x => x.EntrenadorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Cita>(entity =>
        {
            entity.ToTable("Citas");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Motivo).HasMaxLength(300).IsRequired();
            entity.Property(x => x.Estado).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.HasOne(x => x.Entrenador)
                .WithMany(x => x.Citas)
                .HasForeignKey(x => x.EntrenadorId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Pokemon)
                .WithMany(x => x.Citas)
                .HasForeignKey(x => x.PokemonId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.UsuarioAsignado)
                .WithMany(x => x.CitasAsignadas)
                .HasForeignKey(x => x.UsuarioAsignadoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Atencion>(entity =>
        {
            entity.ToTable("Atenciones");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Diagnostico).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Tratamiento).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Observaciones).HasMaxLength(800);
            entity.HasOne(x => x.Cita)
                .WithOne(x => x.Atencion)
                .HasForeignKey<Atencion>(x => x.CitaId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Pokemon)
                .WithMany(x => x.Atenciones)
                .HasForeignKey(x => x.PokemonId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Usuario)
                .WithMany(x => x.AtencionesRealizadas)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ItemInventario>(entity =>
        {
            entity.ToTable("Inventario");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Categoria).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Descripcion).HasMaxLength(400);
        });

        modelBuilder.Entity<Internamiento>(entity =>
        {
            entity.ToTable("Internamientos");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Codigo).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Motivo).HasMaxLength(300).IsRequired();
            entity.Property(x => x.AreaAsignada).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Estado).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.HasIndex(x => x.Codigo).IsUnique();
            entity.HasOne(x => x.Pokemon)
                .WithMany(x => x.Internamientos)
                .HasForeignKey(x => x.PokemonId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.UsuarioResponsable)
                .WithMany(x => x.InternamientosRegistrados)
                .HasForeignKey(x => x.UsuarioResponsableId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TratamientoMedico>(entity =>
        {
            entity.ToTable("TratamientosMedicos");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Tipo).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Dosis).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Frecuencia).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Estado).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.HasOne(x => x.Pokemon)
                .WithMany(x => x.Tratamientos)
                .HasForeignKey(x => x.PokemonId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.UsuarioResponsable)
                .WithMany(x => x.TratamientosRegistrados)
                .HasForeignKey(x => x.UsuarioResponsableId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Internamiento)
                .WithMany()
                .HasForeignKey(x => x.InternamientoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ServicioClinico>(entity =>
        {
            entity.ToTable("ServiciosClinicos");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Codigo).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Descripcion).HasMaxLength(300).IsRequired();
            entity.Property(x => x.CostoBase).HasColumnType("decimal(18,2)");
            entity.HasIndex(x => x.Codigo).IsUnique();
        });

        modelBuilder.Entity<FacturaClinica>(entity =>
        {
            entity.ToTable("FacturasClinicas");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Referencia).HasMaxLength(60).IsRequired();
            entity.Property(x => x.Total).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Estado).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.HasIndex(x => x.Referencia).IsUnique();
            entity.HasOne(x => x.Entrenador)
                .WithMany(x => x.Facturas)
                .HasForeignKey(x => x.EntrenadorId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.UsuarioGenerador)
                .WithMany(x => x.FacturasGeneradas)
                .HasForeignKey(x => x.UsuarioGeneradorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<FacturaDetalle>(entity =>
        {
            entity.ToTable("FacturasDetalle");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.PrecioUnitario).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
            entity.HasOne(x => x.FacturaClinica)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => x.FacturaClinicaId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.ServicioClinico)
                .WithMany(x => x.FacturasDetalle)
                .HasForeignKey(x => x.ServicioClinicoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.ToTable("Auditoria");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Entidad).HasMaxLength(100).IsRequired();
            entity.Property(x => x.EntidadId).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Operacion).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.Property(x => x.Descripcion).HasMaxLength(500).IsRequired();
            entity.HasOne(x => x.Usuario)
                .WithMany(x => x.Auditorias)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ItemInventario>().HasData(
            new ItemInventario
            {
                Id = 1,
                Nombre = "Pocion",
                Categoria = "Medicamento",
                Descripcion = "Recupera puntos de salud.",
                Stock = 25,
                StockMinimo = 5,
                FechaActualizacionUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new ItemInventario
            {
                Id = 2,
                Nombre = "Antidoto",
                Categoria = "Medicamento",
                Descripcion = "Cura envenenamiento.",
                Stock = 18,
                StockMinimo = 4,
                FechaActualizacionUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

        modelBuilder.Entity<ServicioClinico>().HasData(
            new ServicioClinico
            {
                Id = 1,
                Codigo = "CONS",
                Nombre = "Consulta general",
                Descripcion = "Consulta clinica general.",
                CostoBase = 15000m,
                RequiereAprobacionAdministrador = false,
                EsRecurrente = false
            },
            new ServicioClinico
            {
                Id = 2,
                Codigo = "INTN",
                Nombre = "Internamiento diario",
                Descripcion = "Cargo por internamiento diario.",
                CostoBase = 25000m,
                RequiereAprobacionAdministrador = false,
                EsRecurrente = true
            });
    }
}
