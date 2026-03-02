using CesiZen.Api.DTOs;
using CesiZen.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly CesiZenDbContext _db;

    public ArticlesController(CesiZenDbContext db) => _db = db;

    // GET /api/articles?public=true
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetAll([FromQuery] bool? @public)
    {
        var query = _db.Articles.AsNoTracking();

        if (@public.HasValue)
            query = query.Where(a => a.Public == @public.Value);

        var list = await query
            .OrderByDescending(a => a.DatePublication)
            .Select(a => new ArticleDto(
                a.Id,
                a.Titre,
                a.Contenu,
                a.DatePublication,
                a.Public,
                a.CategorieId
            ))
            .ToListAsync();

        return Ok(list);
    }

    // GET /api/articles/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ArticleDto>> GetById(int id)
    {
        var a = await _db.Articles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (a is null) return NotFound();

        return Ok(new ArticleDto(
            a.Id,
            a.Titre,
            a.Contenu,
            a.DatePublication,
            a.Public,
            a.CategorieId
        ));
    }
}