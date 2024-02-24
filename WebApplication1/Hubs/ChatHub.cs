using Microsoft.AspNetCore.Mvc;
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
            _collection = mongoDatabase.GetCollection<User>("Users");
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
        
        public async Task CreateAndEnterDialog(string ownerId, string enterId)
        {
            var existChatRoom = _collectionChatRooms.Find(item => item.Participants[0]._Id == enterId && item.Participants[1]._Id == ownerId).FirstOrDefault();
            if(existChatRoom == null)
            {
                var owner = _collection.Find(item => item._Id == ownerId).FirstOrDefault();
                var entered = _collection.Find(item => item._Id == enterId).FirstOrDefault();

                existChatRoom = new ChatRoom { Participants = [owner, entered] };
                existChatRoom.Name = entered.Name;
                await _collectionChatRooms.InsertOneAsync(existChatRoom);
                await Groups.AddToGroupAsync(Context.ConnectionId, existChatRoom._Id);
                await Clients.Group(existChatRoom._Id).SendAsync("onCreateDialog", existChatRoom);
            }
            else
            {
                await Clients.Group(existChatRoom._Id).SendAsync("onReceiveError", "Current dialog with this user is already exist!");
            }
        }
        public async Task GetDialogs(string ownerId)
        {
            var chatRooms = _collectionChatRooms.Find(item => item.Participants[0]._Id == ownerId || item.Participants[1]._Id ==ownerId).ToList();
            foreach (var chatRoom in chatRooms)
            {
                var secondUserName = chatRoom.Participants[0]._Id ==ownerId ? chatRoom.Participants[1].Name : chatRoom.Participants[0].Name;
                chatRoom.Name = secondUserName;
            }
            await Clients.Caller.SendAsync("onGetDialogs", chatRooms);
        }
       
    }
}
