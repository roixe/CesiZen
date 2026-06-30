using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CesiZen.Api.DTOs;
using CesiZen.Domain.Entities;
using CesiZen.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HistoriquesController : ControllerBase
{
    private readonly CesiZenDbContext _db;

    public HistoriquesController(CesiZenDbContext db)
    {
        _db = db;
    }

    // GET /api/historiques/me
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userIdStr =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userIdStr) || !int.TryParse(userIdStr, out var userId))
            return Unauthorized("Token invalide : userId introuvable.");

        var historiques = await _db.Historiques
            .AsNoTracking()
            .Where(h => h.UtilisateurId == userId)
            .OrderByDescending(h => h.Date)
            .Select(h => new HistoriqueDto(
                h.Id,
                h.UtilisateurId,
                h.Date,
                h.DureeSec,
                h.ExercicesEnregistres
                    .OrderByDescending(e => e.DateDebut)
                    .Select(e => new EnregistreDto(
                        e.Id,
                        e.ExerciceId,
                        e.Exercice.Nom,
                        e.DateDebut,
                        e.DureeEffectiveSec
                    ))
                    .ToList()
            ))
            .ToListAsync();

        return Ok(historiques);
    }

    // POST /api/historiques
    [HttpPost]
    public async Task<IActionResult> Create(CreateHistoriqueDto dto)
    {
        var userIdStr =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userIdStr) || !int.TryParse(userIdStr, out var userId))
            return Unauthorized("Token invalide : userId introuvable.");

        var exerciceExists = await _db.Exercices.AsNoTracking().AnyAsync(e => e.Id == dto.ExerciceId);
        if (!exerciceExists) return BadRequest("Exercice invalide.");

        var historique = new Historique
        {
            UtilisateurId = userId,
            Date = DateTime.UtcNow,
            DureeSec = dto.DureeEffectiveSec
        };

        _db.Historiques.Add(historique);
        await _db.SaveChangesAsync();

        var enregistrement = new Enregistre
        {
            HistoriqueId = historique.Id,
            ExerciceId = dto.ExerciceId,
            DateDebut = DateTime.UtcNow,
            DureeEffectiveSec = dto.DureeEffectiveSec
        };

        _db.Enregistrements.Add(enregistrement);
        await _db.SaveChangesAsync();

        return Ok(new { id = historique.Id });
    }
}