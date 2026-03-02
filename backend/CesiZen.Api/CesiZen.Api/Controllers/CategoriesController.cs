using CesiZen.Api.DTOs;
using CesiZen.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly CesiZenDbContext _db;

    public CategoriesController(CesiZenDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategorieDto>>> GetAll()
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Nom)
            .Select(c => new CategorieDto(c.Id, c.Nom))
            .ToListAsync();

        return Ok(categories);
    }
}