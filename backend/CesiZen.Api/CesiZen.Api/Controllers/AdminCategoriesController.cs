using CesiZen.Api.DTOs.Admin;
using CesiZen.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.Api.Controllers;

[Authorize(Roles = "ADMIN")]
[ApiController]
[Route("api/admin/categories")]
public class AdminCategoriesController : ControllerBase
{
    private readonly CesiZenDbContext _db;
    public AdminCategoriesController(CesiZenDbContext db) => _db = db;

    // GET /api/admin/categories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategorieDto>>> GetAll()
    {
        var list = await _db.Categories.AsNoTracking()
            .OrderBy(c => c.Nom)
            .Select(c => new CategorieDto(c.Id, c.Nom))
            .ToListAsync();

        return Ok(list);
    }

    // POST /api/admin/categories
    [HttpPost]
    public async Task<ActionResult<CategorieDto>> Create(UpsertCategorieDto dto)
    {
        var nom = dto.Nom.Trim();
        if (nom.Length < 2) return BadRequest("Nom trop court.");

        var exists = await _db.Categories.AnyAsync(c => c.Nom.ToLower() == nom.ToLower());
        if (exists) return Conflict("Categorie déjà existante.");

        var cat = new Domain.Entities.Categorie { Nom = nom };
        _db.Categories.Add(cat);
        await _db.SaveChangesAsync();

        return Ok(new CategorieDto(cat.Id, cat.Nom));
    }

    // PUT /api/admin/categories/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpsertCategorieDto dto)
    {
        var nom = dto.Nom.Trim();
        if (nom.Length < 2) return BadRequest("Nom trop court.");

        var cat = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (cat is null) return NotFound();

        cat.Nom = nom;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // DELETE /api/admin/categories/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (cat is null) return NotFound();

        // Empêcher suppression si des articles l’utilisent
        var used = await _db.Articles.AsNoTracking().AnyAsync(a => a.CategorieId == id);
        if (used) return Conflict("Categorie utilisée par des articles.");

        _db.Categories.Remove(cat);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}