using AV.Engine.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AV.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnDemandController : ControllerBase
    {
        private readonly IAVEngine _engine;
        public OnDemandController(IAVEngine engine) => _engine = engine;

        [HttpPost("start")]
        public async Task<IActionResult> Start()
        {
            var result = await _engine.StartScanAsync();
            return result == StartScanResult.Success
                ? Ok()
                : Conflict(new { error = "Scan already running" });
        }

        [HttpPost("stop")]
        public IActionResult Stop()
        {
            _engine.StopScanAsync();
            return Ok();
        }
    }
}
