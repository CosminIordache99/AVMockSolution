using AV.Engine.Core.Enums;
using AV.Engine.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AV.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnDemandController : ControllerBase
    {
        private readonly IAVEngine _engine;
        private readonly ILogger<OnDemandController> _logger;

        public OnDemandController(IAVEngine engine, ILogger<OnDemandController> logger)
        {
            _engine = engine;
            _logger = logger;
        }

        /// <summary>Start an on-demand scan.</summary>
        [HttpPost("start")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Start(CancellationToken ct)
        {
            var result = await _engine.StartOnDemandScanAsync();
            _logger.LogInformation("StartScanAsync → {Result}", result);
            return result == ScanResult.Success
                ? Ok()
                : Conflict(new { error = "Scan already in progress" });
        }

        /// <summary>Stop an on-demand scan.</summary>
        [HttpPost("stop")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Stop(CancellationToken ct)
        {
            var stopped = await _engine.StopOnDemandScanAsync();
            _logger.LogInformation("StopScanAsync → {Stopped}", stopped);
            return Ok();
        }
    }
}
