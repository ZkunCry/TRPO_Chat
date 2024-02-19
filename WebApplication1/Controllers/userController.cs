using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebApplication1.jwthandler;
using WebApplication1.UserService;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly JwtToken jwtTokenGenerator;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
            
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username,string password)
        {
            var existringUser = await _userService.GetUserByUsername(username);
            if (existringUser == null)
            {
                return BadRequest("Such user does not exist");
            }
            _logger.LogInformation("Check password user: {0} ", password);
            var isCorrect = Hash.ComparePasswords(password, existringUser.Password);
            _logger.LogInformation("Result comparepass: {0}", isCorrect);
            if (isCorrect)
            {
                var accessToken = JwtToken.GenerateToken(username);
                var result = new
                {
                    existringUser,
                    accessToken
                };
                return Ok(result);
            }
            return BadRequest("Incorrect password");


        }
        [HttpPost]
        public async Task<IActionResult> Register(string username,string password)
        {
         
            var existingUser = await _userService.GetUserByUsername(username);
            _logger.LogInformation("exist:{0} ", existingUser);
            if (existingUser != null)
            {
                return BadRequest("User already exists");
            }
            _logger.LogInformation("Existing user: {0} ",existingUser);
            var hashPass = Hash.ComputeSHA256Hash(password);
            _logger.LogInformation("Creating hash for pass: {0}",hashPass);
            var user = new User { Name = username, Password = hashPass };

            _logger.LogInformation("User: {0}", user);

            await _userService.CreateUser(new User { Name = username, Password = hashPass });
            _logger.LogInformation("Created user {0} ", (await _userService.GetUserByUsername(username)).ToString());
            var response =  await _userService.GetUserByUsername(username);
  

            return Ok(response);
        }
        [HttpGet]
        public async  Task<string> GetUserByName(string name)
        {
            var result = await _userService.GetUserByUsername(name);
            _logger.LogInformation("User: {0}", result._Id);
            return result.ToJson();
        }


    }
}
