using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Route("[controller]/action")]
    [ApiController]
    public class messageController : ControllerBase
    {
        private readonly ILogger<messageController> _logger;

        public messageController(ILogger<messageController> logger)
        {
            _logger = logger;
        }
        [HttpGet(Name ="getMessages")]
        public IActionResult Get()
        {

        }

    }
}
