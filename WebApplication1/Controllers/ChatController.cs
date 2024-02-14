using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        
        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "enterChat")]
        public async Task<ActionResult<Chat>> EnterChat(int chatRoomId, string nameChat,string userName)
        {
            if (chatRoomId == 0 || string.IsNullOrWhiteSpace(nameChat)) {
                return BadRequest(new { message = "Incorrect data" });
            }
            /*Random random = new Random();
            var currentUser = new User { Id = random.Next(), Name = userName };
            Program.users.Add(new User { Id = random.Next(), Name = userName });
            var currentChat = Program.chats.Find(chat=>chat.chatroomId ==chatRoomId && chat.chatRoomName == nameChat);
            currentChat.users.Add(currentUser);*/
            return Ok();

        }
        [HttpPost(Name = "createChat")]
        public async Task<ActionResult<Chat>> createChat(string nameCreatedChat)
        {
           if(nameCreatedChat == null) 
            {
                return BadRequest(new { message = "Name chat is empty" });
            }
          var random = new Random();
          var chat = new Chat { chatRoomName = nameCreatedChat, chatroomId = random.Next() };
       /*   Program.chats.Add(new Chat { chatRoomName = nameCreatedChat, chatroomId =random.Next()  });*/
          return Ok(new {Chat = chat});
        }
    }
}
