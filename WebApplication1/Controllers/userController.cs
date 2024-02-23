using Microsoft.AspNetCore.Authorization;
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
            var user = await _userService.GetUserByUsername(username);
            if (user == null)
            {
                return BadRequest("Such user does not exist");
            }
            _logger.LogInformation("Check password user: {0} ", password);
            var isCorrect = Hash.ComparePasswords(password, user.Password);
            _logger.LogInformation("Result comparepass: {0}", isCorrect);
            _logger.LogInformation("User id {0}", user._Id);
            if (isCorrect)
            {
                var accessToken = JwtToken.GenerateToken(username,user._Id);
                var result = new
                {
                    user._Id,
                    user.Name,
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

            await _userService.CreateUser(user);
            _logger.LogInformation("Created user {0} ", (await _userService.GetUserByUsername(username)).ToString());
            var accessToken = JwtToken.GenerateToken(username, user._Id);
            var response = new
            {
                user._Id,
                user.Name,
                accessToken
            };

            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetUserByName(string name)
        {
            var result = await _userService.GetUserByUsername(name);
            _logger.LogInformation("User: {0}", result._Id);
            return Ok(result);
        }


    }
}
