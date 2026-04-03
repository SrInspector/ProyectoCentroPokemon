using CentroPokemon.BC.DTOs.Auth;
using CentroPokemon.BC.Entidades;
using CentroPokemon.BC.Enumeradores;
using CentroPokemon.BC.Seguridad;
using CentroPokemon.BW.Interfaces.BW;
using CentroPokemon.BW.Interfaces.DA;

namespace CentroPokemon.BW.CU;

public class AutenticacionBW : IAutenticacionBW
{
    private readonly IUsuarioDA _usuarioDA;
    private readonly ITokenDA _tokenDA;
    private readonly IAuditoriaBW _auditoriaBW;
    private readonly IEntrenadorDA _entrenadorDA;

    public AutenticacionBW(IUsuarioDA usuarioDA, ITokenDA tokenDA, IAuditoriaBW auditoriaBW, IEntrenadorDA entrenadorDA)
    {
        _usuarioDA = usuarioDA;
        _tokenDA = tokenDA;
        _auditoriaBW = auditoriaBW;
        _entrenadorDA = entrenadorDA;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        ValidarCredenciales(request.Correo, request.Password);

        var usuario = await _usuarioDA.ObtenerPorCorreoAsync(request.Correo.Trim().ToLowerInvariant())
            ?? throw new InvalidOperationException("Las credenciales son invalidas.");

        if (usuario.BloqueadoHastaUtc.HasValue && usuario.BloqueadoHastaUtc.Value > DateTime.UtcNow)
        {
            throw new InvalidOperationException("La cuenta esta bloqueada temporalmente. Intente de nuevo mas tarde.");
        }

        if (!PasswordHasherBC.Verify(request.Password, usuario.PasswordHash))
        {
            usuario.IntentosFallidos++;
            if (usuario.IntentosFallidos >= 5)
            {
                usuario.BloqueadoHastaUtc = DateTime.UtcNow.AddMinutes(15);
                usuario.IntentosFallidos = 0;
            }

            await _usuarioDA.ActualizarAsync(usuario);
            await _auditoriaBW.RegistrarAsync(usuario.Id, "Usuario", usuario.Id.ToString(), TipoOperacionAuditoria.LoginFallido, "Intento de inicio de sesion fallido.");
            throw new InvalidOperationException("Las credenciales son invalidas.");
        }

        usuario.IntentosFallidos = 0;
        usuario.BloqueadoHastaUtc = null;
        usuario.UltimoAccesoUtc = DateTime.UtcNow;
        await _usuarioDA.ActualizarAsync(usuario);
        await _auditoriaBW.RegistrarAsync(usuario.Id, "Usuario", usuario.Id.ToString(), TipoOperacionAuditoria.LoginExitoso, "Inicio de sesion exitoso.");
        return CrearRespuesta(usuario);
    }

    public async Task<AuthResponse> CrearUsuarioAsync(CrearUsuarioRequest request)
    {
        ValidarCredenciales(request.Correo, request.Password);

        if (string.IsNullOrWhiteSpace(request.NombreCompleto))
        {
            throw new InvalidOperationException("El nombre completo es requerido.");
        }

        var correoNormalizado = request.Correo.Trim().ToLowerInvariant();
        if (await _usuarioDA.ObtenerPorCorreoAsync(correoNormalizado) is not null)
        {
            throw new InvalidOperationException("Ya existe un usuario registrado con ese correo.");
        }

        if (request.Rol == BC.Enumeradores.RolSistema.Entrenador)
        {
            if (!request.EntrenadorId.HasValue)
            {
                throw new InvalidOperationException("Debe indicar el entrenador asociado para usuarios con rol entrenador.");
            }

            if (await _entrenadorDA.ObtenerPorIdAsync(request.EntrenadorId.Value) is null)
            {
                throw new InvalidOperationException("El entrenador asociado no existe.");
            }
        }

        var usuario = new Usuario
        {
            NombreCompleto = request.NombreCompleto.Trim(),
            Correo = correoNormalizado,
            PasswordHash = PasswordHasherBC.Hash(request.Password),
            Rol = request.Rol,
            EntrenadorId = request.EntrenadorId
        };

        await _usuarioDA.CrearAsync(usuario);
        await _auditoriaBW.RegistrarAsync(usuario.Id, "Usuario", usuario.Id.ToString(), TipoOperacionAuditoria.Crear, "Usuario registrado en el sistema.");
        return CrearRespuesta(usuario);
    }

    private AuthResponse CrearRespuesta(Usuario usuario)
    {
        return new AuthResponse
        {
            Token = _tokenDA.GenerarToken(usuario),
            UsuarioId = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Correo = usuario.Correo,
            Rol = usuario.Rol
        };
    }

    private static void ValidarCredenciales(string correo, string password)
    {
        if (string.IsNullOrWhiteSpace(correo) || !correo.Contains('@'))
        {
            throw new InvalidOperationException("El correo no es valido.");
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            throw new InvalidOperationException("La contrasena debe tener al menos 8 caracteres.");
        }

        if (!password.Any(char.IsUpper) || !password.Any(char.IsDigit) || !password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            throw new InvalidOperationException("La contrasena debe incluir al menos una mayuscula, un numero y un simbolo.");
        }
    }
}
