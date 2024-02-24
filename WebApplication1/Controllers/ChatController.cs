using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
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
        private readonly IMongoCollection<ChatRoom> _collection;
        private readonly IMongoCollection<User> _collectionUsers;
        public ChatController(IMongoDatabase database, IHubContext<ChatHub> chatHubContext, IChatRoomService chatRoomService, ILogger<ChatController> logger)
        {
            _logger = logger;
            _collection = database.GetCollection<ChatRoom>("ChatRooms");
            _collectionUsers = database.GetCollection<User>("Users");
            _chatHubContext = chatHubContext;
            _chatRoomService = chatRoomService;
        }
        [HttpPost]
        public async Task SendMessageToRoom(string roomId, string senderId, string messageText)
        {
            await _chatRoomService.SendMessage(roomId, senderId, messageText);
            await _chatHubContext.Clients.All.SendAsync("ReceiveMessage", roomId.ToString(), senderId, messageText);
        }
        [HttpPost]
        public async Task<IActionResult> EnterChatRoom(string joinerId, string chatRoomId)
        {
            var chatRoom = _collection.Find(c => c._Id == chatRoomId).FirstOrDefault();
            var userEnter = _collectionUsers.Find(user => user._Id == joinerId).FirstOrDefault();
            _logger.LogInformation("Entered user: {0}", userEnter.Name);
            if (chatRoom != null)
            {
                if (chatRoom.sender == null)
                {
                    chatRoom.sender = userEnter._Id;

                    var filter = Builders<ChatRoom>.Filter.Eq("_Id", chatRoom._Id);
                    var update = Builders<ChatRoom>.Update.Set("SenderId", ObjectId.Parse( userEnter._Id));
                    _collection.UpdateOne(filter, update);
                    await _chatHubContext.Clients.Group(chatRoom.Name).SendAsync("JoinRoom", chatRoomId);
                    return Ok($"{userEnter.Name} entered the chat room {chatRoom.Name} successfully.");
                }
                else
                {
                    return BadRequest($"{userEnter.Name} is already in the chat room.");
                }
            }
            else
            {
                return NotFound("Chat room not found.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateChatRoom(string userId, string roomName)
        {
            var user = _collectionUsers.Find(item => item._Id == userId).FirstOrDefault();
            if(user == null) 
            {
                return BadRequest($"User with id = {userId} doesn't exist.");
            }
            var chatRoom = new ChatRoom { Name = roomName,owner =user._Id };
            await _collection.InsertOneAsync(chatRoom);
            await _chatHubContext.Clients.Group(roomName).SendAsync("UserEntered", user.Name);
            return Ok($"Chat room {roomName} created successfully for user {user.Name}.");
        }

    }
}
