using Microsoft.AspNetCore.Mvc;

namespace OrderIngest.Api.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }
}
