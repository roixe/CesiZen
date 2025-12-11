using Microsoft.AspNetCore.Mvc;

namespace CesiZen.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new { status = "ok", backend = "CesiZen.Api" });
    }
}
