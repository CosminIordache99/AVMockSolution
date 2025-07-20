using AV.API.DTOs;
using AV.Engine.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AV.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IAVEngine _engine;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IAVEngine engine, ILogger<EventsController> logger)
        {
            _engine = engine;
            _logger = logger;
        }

        /// <summary>Get completed scan sessions, each with its start/stop and any threats.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ScanSessionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ScanSessionDto>>> Get(CancellationToken ct)
        {
            var events = await _engine.GetPersistedEventsAsync();
            var sessions = ScanSessionMapper.ToScanResults(events);
            _logger.LogInformation(message: "Returning {Count} sessions");
            return Ok(sessions);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Clear(CancellationToken ct)
        {
            await _engine.ClearPersistedEventsAsync();
            _logger.LogInformation("Cleared all events");
            return NoContent();
        }
    }
}
