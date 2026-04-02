using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CesiZen.Api.DTOs;
using CesiZen.Api.DTOs.Admin;
using CesiZen.Domain.Entities;
using CesiZen.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.Api.Controllers;

[Authorize(Roles = "ADMIN")]
[ApiController]
[Route("api/admin/articles")]
public class AdminArticlesController : ControllerBase
{
    private readonly CesiZenDbContext _db;

    public AdminArticlesController(CesiZenDbContext db) => _db = db;

    // GET /api/admin/articles (liste tout, public ou pas)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetAll()
    {
        var list = await _db.Articles.AsNoTracking()
            .OrderByDescending(a => a.DatePublication)
            .Select(a => new ArticleDto(
                a.Id, a.Titre, a.Contenu, a.DatePublication, a.Public, a.CategorieId
            ))
            .ToListAsync();

        return Ok(list);
    }

    // POST /api/admin/articles
    [HttpPost]
    public async Task<ActionResult<ArticleDto>> Create(UpsertArticleDto dto)
    {
        var userIdStr =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userIdStr) || !int.TryParse(userIdStr, out var userId))
            return Unauthorized("Token invalide : userId introuvable.");

        //valider que la catégorie existe
        var catExists = await _db.Categories.AsNoTracking().AnyAsync(c => c.Id == dto.CategorieId);
        if (!catExists) return BadRequest("Categorie invalide.");

        var article = new Article
        {
            Titre = dto.Titre.Trim(),
            Contenu = dto.Contenu,
            CategorieId = dto.CategorieId,
            Public = dto.Public,
            DatePublication = dto.Public ? DateTime.UtcNow : null,
            GereParUserId = userId
        };

        _db.Articles.Add(article);
        await _db.SaveChangesAsync();

        return Ok(new ArticleDto(
            article.Id, article.Titre, article.Contenu, article.DatePublication, article.Public, article.CategorieId
        ));
    }

    // PUT /api/admin/articles/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpsertArticleDto dto)
    {
        var article = await _db.Articles.FirstOrDefaultAsync(a => a.Id == id);
        if (article is null) return NotFound();

        var catExists = await _db.Categories.AsNoTracking().AnyAsync(c => c.Id == dto.CategorieId);
        if (!catExists) return BadRequest("Categorie invalide.");

        article.Titre = dto.Titre.Trim();
        article.Contenu = dto.Contenu;
        article.CategorieId = dto.CategorieId;

        // gestion publish/unpublish + date
        if (article.Public == false && dto.Public == true)
            article.DatePublication = DateTime.UtcNow; // 1ère publication

        if (dto.Public == false)
            article.DatePublication = null; // dépublication 

        article.Public = dto.Public;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/admin/articles/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var article = await _db.Articles.FirstOrDefaultAsync(a => a.Id == id);
        if (article is null) return NotFound();

        _db.Articles.Remove(article);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}