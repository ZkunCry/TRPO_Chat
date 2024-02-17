using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using WebApplication1.ChatRoomService;
using WebApplication1.Hubs;


namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly IChatRoomService _chatRoomService;

        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
        }
        public ChatController(IHubContext<ChatHub> chatHubContext, IChatRoomService chatRoomService)
        {
            _chatHubContext = chatHubContext;
            _chatRoomService = chatRoomService;
        }
        [HttpPost]
        public async Task SendMessageToRoom(ObjectId roomId, string senderId, string messageText)
        {
            await _chatRoomService.SendMessage(roomId, senderId, messageText);
            await _chatHubContext.Clients.All.SendAsync("ReceiveMessage", roomId.ToString(), senderId, messageText);
        }


    }
}
