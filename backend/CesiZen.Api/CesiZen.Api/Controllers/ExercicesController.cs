using CesiZen.Api.DTOs;
using CesiZen.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExercicesController : ControllerBase
{
    private readonly CesiZenDbContext _db;

    public ExercicesController(CesiZenDbContext db) => _db = db;

    // GET /api/exercices?type=RESPIRATION&public=true
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExerciceDto>>> GetAll([FromQuery] string? type, [FromQuery] bool? @public)
    {
        var query = _db.Exercices.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(e => e.Type == type);

        if (@public.HasValue)
            query = query.Where(e => e.Public == @public.Value);

        var list = await query
            .OrderBy(e => e.Nom)
            .Select(e => new ExerciceDto(
                e.Id,
                e.Nom,
                e.Type,
                e.Description,
                e.Public,
                e.InspireSec,
                e.ApneeSec,
                e.ExpireSec,
                e.Apnee2Sec,
                e.Cycles,
                e.DureeTotaleSec
            ))
            .ToListAsync();

        return Ok(list);
    }

    // GET /api/exercices/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExerciceDto>> GetById(int id)
    {
        var e = await _db.Exercices.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (e is null) return NotFound();

        return Ok(new ExerciceDto(
            e.Id,
            e.Nom,
            e.Type,
            e.Description,
            e.Public,
            e.InspireSec,
            e.ApneeSec,
            e.ExpireSec,
            e.Apnee2Sec,
            e.Cycles,
            e.DureeTotaleSec
        ));
    }
}