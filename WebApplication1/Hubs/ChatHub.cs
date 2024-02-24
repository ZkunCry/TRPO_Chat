using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using WebApplication1.Controllers;
using WebApplication1.jwthandler;


namespace WebApplication1.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMongoCollection<Message> messages;
        private readonly IMongoCollection<ChatRoom> _collectionChatRooms;
        private readonly IMongoCollection<User> _collection;

        private readonly ILogger<ChatHub> logger;
        public ChatHub(IMongoDatabase mongoDatabase, ILogger<ChatHub> logger)
        {
            messages = mongoDatabase.GetCollection<Message>("Messages");
            _collectionChatRooms = mongoDatabase.GetCollection<ChatRoom>("ChatRooms");
            this.logger = logger;
        }
        public async Task SendMessage(string chatRoomId,string message,string senderId)
        {
            logger.LogInformation("ChatroomId {0} message {1} senderId {2}",chatRoomId,message,senderId);  
            var newMessage = new Message { chatRoomId = chatRoomId, Text = message,SenderId = senderId };
            await messages.InsertOneAsync(newMessage);
            await Clients.Group(chatRoomId).SendAsync("onMessage",newMessage);
        }
        public override async Task OnConnectedAsync()
        {
            var documents = await _collectionChatRooms.Find(new BsonDocument()).ToListAsync();
            foreach (var chatRoom in documents)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chatRoom._Id);
            }

           await  base.OnConnectedAsync();
        }
        public async Task CreateDialog(string senderId, string chatRoomId)
        {
            var chatRoom = _collectionChatRooms.Find(c => c._Id == chatRoomId).FirstOrDefault();
            var userEnter = _collection.Find(user => user._Id == senderId).FirstOrDefault();
            logger.LogInformation("Entered user: {0}", userEnter.Name);
            if (chatRoom != null)
            {
                if (chatRoom.sender == null)
                {
                    chatRoom.sender = userEnter._Id;
                    var filter = Builders<ChatRoom>.Filter.Eq("_Id", chatRoom._Id);
                    var update = Builders<ChatRoom>.Update.Set("SenderId", ObjectId.Parse(userEnter._Id));
                    _collectionChatRooms.UpdateOne(filter, update);
                    await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId);
                    await Clients.Group(chatRoom._Id).SendAsync("onCreateDialog", chatRoom);
                   
                }
                else
                {
                    await Clients.Group(chatRoomId).SendAsync("onReceiveError", $"User {userEnter.Name} is already exist");
                }
            }
            else
            {
                await Clients.Group(chatRoomId).SendAsync("onReceiveError", $"Chatroom with id {chatRoomId} not found");

            }
        }
    }
}
