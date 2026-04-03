using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CentroPokemon.BC.Entidades;
using CentroPokemon.BW.Interfaces.DA;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CentroPokemon.DA.Seguridad;

public class TokenDA : ITokenDA
{
    private readonly IConfiguration _configuration;

    public TokenDA(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerarToken(Usuario usuario)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("La llave JWT no esta configurada.");
        var issuer = _configuration["Jwt:Issuer"] ?? "CentroPokemon.API";
        var audience = _configuration["Jwt:Audience"] ?? "CentroPokemon.Cliente";

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Correo),
            new(ClaimTypes.Name, usuario.NombreCompleto),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Role, usuario.Rol.ToString())
        };

        if (usuario.EntrenadorId.HasValue)
        {
            claims.Add(new("entrenadorId", usuario.EntrenadorId.Value.ToString()));
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
