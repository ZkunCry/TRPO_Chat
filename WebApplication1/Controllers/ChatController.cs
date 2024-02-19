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
        [HttpPost("enterroom")]
        public async Task<IActionResult> EnterChatRoom(string username, string chatRoomName)
        {
            var chatRoom = _collection.Find(c => c.Name == chatRoomName).FirstOrDefault();
            var userEnter = _collectionUsers.Find(user => user.Name == username).FirstOrDefault();
            if (chatRoom != null)
            {
                if (!chatRoom.Participants.Contains(new User {Name = username }))
                {
                
                    chatRoom.Participants.Add(userEnter);
                    var filter = Builders<ChatRoom>.Filter.Eq("_id", chatRoom._Id);
                    var update = Builders<ChatRoom>.Update.Set("Participants", chatRoom.Participants);
                    _collection.UpdateOne(filter, update);

                    await _chatHubContext.Clients.Group(chatRoomName).SendAsync("UserEntered", username);

                    return Ok($"{username} entered the chat room {chatRoomName} successfully.");
                }
                else
                {
                    return BadRequest($"{username} is already in the chat room.");
                }
            }
            else
            {
                return NotFound("Chat room not found.");
            }
        }
        [HttpPost("createroom")]
        public async Task<IActionResult> CreateChatRoom(string username, string roomName)
        {
            var user = _collectionUsers.Find(item => item.Name == username).FirstOrDefault();
            if(user == null) 
            {
                return BadRequest($"{username} doesn't exist.");
            }
            var chatRoom = new ChatRoom { Name = roomName, Participants = new List<User>() };
            chatRoom.Participants.Add(user);
            await _collection.InsertOneAsync(chatRoom);

            await _chatHubContext.Clients.Group(roomName).SendAsync("UserEntered", username);

            return Ok($"Chat room {roomName} created successfully for user {username}.");
        }

    }
}
