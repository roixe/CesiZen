using CesiZen.Domain.Entities;
using CesiZen.Infrastructure.Data;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Utilisateurs.ToListAsync();
            return Ok(users);
        }
    }
}
