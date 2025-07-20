using AV.Engine.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AV.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealTimeController : ControllerBase
    {
        private readonly IAVEngine _engine;
        public RealTimeController(IAVEngine engine) => _engine = engine;

        [HttpPost("enable")]
        public IActionResult Enable()
        {
            _engine.EnableRealTime();
            return Ok();
        }

        [HttpPost("disable")]
        public IActionResult Disable()
        {
            _engine.DisableRealTime();
            return Ok();
        }
    }
}
