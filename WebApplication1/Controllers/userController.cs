using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]


    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name ="SendMessage")]
        public async Task<ActionResult<User>> sendMessage(string message)
        {
          if(message == null || string.IsNullOrWhiteSpace(message)) 
          {
                return BadRequest("String is empty!");

           }
            return Ok();
        }

    }
}
