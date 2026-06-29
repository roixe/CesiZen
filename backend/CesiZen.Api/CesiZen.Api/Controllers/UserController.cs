using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CesiZen.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly CesiZenDbContext _context;

        public UserController(CesiZenDbContext context)
        {
            _context = context;
        }

        // Liste des utilisateurs — réservée aux administrateurs.
        // [SÉCU] Auparavant accessible sans authentification ET renvoyait le hash : corrigé.
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Utilisateurs
                .AsNoTracking()
                .Select(u => new { u.Id, u.Nom, u.Email, u.Role, u.Actif, u.DateCreation })
                .ToListAsync();
            return Ok(users);
        }

        // [SÉCU 3 / RGPD] Export de ses propres données (droit à la portabilité).
        [Authorize]
        [HttpGet("me/export")]
        public async Task<IActionResult> ExportMyData()
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var user = await _context.Utilisateurs.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null) return NotFound();

            var historiques = await _context.Historiques.AsNoTracking()
                .Where(h => h.UtilisateurId == userId)
                .OrderByDescending(h => h.Date)
                .Select(h => new { h.Id, h.Date, h.DureeSec })
                .ToListAsync();

            var export = new
            {
                profil = new
                {
                    user.Id,
                    user.Nom,
                    user.Email,
                    user.Role,
                    user.DateCreation,
                    user.DateConsentement
                },
                historiques
            };

            return Ok(export);
        }

        // [SÉCU 3 / RGPD] Suppression de son propre compte (droit à l'effacement).
        [Authorize]
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMyAccount()
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null) return NotFound();

            // Supprime d'abord les données liées (historiques + enregistrements)
            var histos = await _context.Historiques
                .Where(h => h.UtilisateurId == userId)
                .ToListAsync();
            var histoIds = histos.Select(h => h.Id).ToList();

            var enrs = await _context.Enregistrements
                .Where(e => histoIds.Contains(e.HistoriqueId))
                .ToListAsync();

            _context.Enregistrements.RemoveRange(enrs);
            _context.Historiques.RemoveRange(histos);
            _context.Utilisateurs.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TryGetUserId(out int userId)
        {
            var s = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                    ?? User.FindFirstValue("sub");
            return int.TryParse(s, out userId);
        }
    }
}
