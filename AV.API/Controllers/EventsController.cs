using AV.Engine.Core.Entities;
using AV.Engine.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AV.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IAVEngine _engine;
        public EventsController(IAVEngine engine) => _engine = engine;

        public record ThreatInfoDto(string FilePath, string ThreatName);
        public record ScanSessionDto(
            DateTime StartTimestamp,
            DateTime StopTimestamp,
            string Reason,
            IEnumerable<ThreatInfoDto> Threats
        );

        [HttpGet]
        public IActionResult GetAll()
        {
            var events = _engine.GetPersistedEvents().ToList();
            var sessions = new List<ScanSessionDto>();

            DateTime? startTime = null;
            var currentThreats = new List<ThreatInfoDto>();

            foreach (var e in events)
            {
                switch (e)
                {
                    case ScanStartedEvent se:
                        startTime = se.Timestamp;
                        currentThreats.Clear();
                        break;

                    case ThreatsFoundEvent te when startTime.HasValue:
                        foreach (var t in te.Threats)
                        {
                            currentThreats.Add(new ThreatInfoDto(t.FilePath, t.ThreatName));
                        }
                        break;

                    case ScanStoppedEvent se when startTime.HasValue:
                        sessions.Add(new ScanSessionDto(
                            StartTimestamp: startTime.Value,
                            StopTimestamp: se.Timestamp,
                            Reason: se.Reason,
                            Threats: new List<ThreatInfoDto>(currentThreats)
                        ));
                        startTime = null;
                        currentThreats.Clear();
                        break;
                }
            }

            return Ok(sessions);
        }

        [HttpDelete]
        public IActionResult Clear()
        {
            _engine.ClearPersistedEvents();
            return NoContent();
        }
    }
}
