using CesiZen.Api.DTOs;
using CesiZen.Domain.Entities;
using CesiZen.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HistoriquesController : ControllerBase
{
    private readonly CesiZenDbContext _db;

    public HistoriquesController(CesiZenDbContext db)
    {
        _db = db;
    }

    // POST /api/historiques

[HttpGet("user/{userId:int}")]
public async Task<IActionResult> GetByUser(int userId)
{
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
}