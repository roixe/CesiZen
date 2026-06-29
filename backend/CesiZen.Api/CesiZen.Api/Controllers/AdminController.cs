using CesiZen.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.Api.Controllers;

[Authorize(Roles = "ADMIN")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly CesiZenDbContext _db;

    public AdminController(CesiZenDbContext db)
    {
        _db = db;
    }

    // GET /api/admin/users
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _db.Utilisateurs
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .Select(u => new
            {
                u.Id,
                u.Nom,
                u.Email,
                u.Role,
                u.Actif,
                u.DateCreation
            })
            .ToListAsync();

        return Ok(users);
    }

    // PUT /api/admin/users/{id}/disable
    [HttpPut("users/{id:int}/disable")]
    public async Task<IActionResult> DisableUser(int id)
    {
        var user = await _db.Utilisateurs.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound();

        

        user.Actif = false;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // PUT /api/admin/users/{id}/enable (pratique)
    [HttpPut("users/{id:int}/enable")]
    public async Task<IActionResult> EnableUser(int id)
    {
        var user = await _db.Utilisateurs.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound();

        user.Actif = true;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // PUT /api/admin/users/{id}/role
    [HttpPut("users/{id:int}/role")]
    public async Task<IActionResult> SetRole(int id, [FromBody] string role)
    {
        role = role.Trim().ToUpperInvariant();
        if (role is not ("ADMIN" or "USER")) return BadRequest("Role invalide (ADMIN|USER).");

        var user = await _db.Utilisateurs.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound();

        user.Role = role;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}