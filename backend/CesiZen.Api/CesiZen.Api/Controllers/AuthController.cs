using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CesiZen.Api.DTOs.Auth;
using CesiZen.Domain.Entities;
using CesiZen.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace CesiZen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly CesiZenDbContext _db;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthController(CesiZenDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var exists = await _db.Utilisateurs.AsNoTracking().AnyAsync(u => u.Email == email);
        if (exists) return Conflict("Email déjà utilisé.");

        var user = new User
        {
            Nom = dto.Nom.Trim(),
            Email = email,
            Role = "USER",
            Actif = true,
            DateCreation = DateTime.UtcNow
        };

        user.MotDePasseHash = _passwordHasher.HashPassword(user, dto.Password);

        _db.Utilisateurs.Add(user);
        await _db.SaveChangesAsync();

        var token = GenerateJwt(user);

        return Ok(new AuthResponseDto(
            token,
            new AuthUserDto(user.Id, user.Nom, user.Email, user.Role)
        ));
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var user = await _db.Utilisateurs.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null) return Unauthorized("Identifiants invalides.");
        if (!user.Actif) return Unauthorized("Compte désactivé.");

        var result = _passwordHasher.VerifyHashedPassword(user, user.MotDePasseHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            return Unauthorized("Identifiants invalides.");

        var token = GenerateJwt(user);

        return Ok(new AuthResponseDto(
            token,
            new AuthUserDto(user.Id, user.Nom, user.Email, user.Role)
        ));
    }

    private string GenerateJwt(User user)
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString());
        new Claim(ClaimTypes.Role, user.Role);
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];
        var key = _config["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Jwt:Key manquant dans la configuration.");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new("name", user.Nom)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(6),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}