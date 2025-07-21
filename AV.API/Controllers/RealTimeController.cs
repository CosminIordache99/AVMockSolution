using AV.Engine.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AV.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealTimeController : ControllerBase
    {
        private readonly IAVEngine _engine;
        private readonly ILogger<RealTimeController> _logger;

        public RealTimeController(IAVEngine engine, ILogger<RealTimeController> logger)
        {
            _engine = engine;
            _logger = logger;
        }

        [HttpPost("enable")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Enable(CancellationToken ct)
        {
            await _engine.EnableRealTimeAsync();
            _logger.LogInformation("Real-time scanning enabled");
            return Ok();
        }

        [HttpPost("disable")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Disable(CancellationToken ct)
        {
            await _engine.DisableRealTimeAsync();
            _logger.LogInformation("Real-time scanning disabled");
            return Ok();
        }
    }
}
