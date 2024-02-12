using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;

        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "enterChat")]

        public async Task<ActionResult<Chat>> EnterChat(int chatRoomId, string nameChat)
        {
            if (chatRoomId == 0 || string.IsNullOrWhiteSpace(nameChat)) {
                return BadRequest(new { message = "Incorrect data" });
            }
            Random random = new Random();
            User[] users = new User[5]; 

            users = Enumerable.Range(1, 5).Select(index =>new User { Id = index, Name = "fdsfsd" }).ToArray();
            return Ok(new Chat
            { chatroomId = random.Next(), chatRoomName = "КПСС", users = users });
        }

    }
}
