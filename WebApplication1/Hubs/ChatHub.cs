using Microsoft.AspNetCore.SignalR;
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
        private readonly IMongoCollection<ChatRoom> chatrooms;
        private readonly ILogger<ChatHub> logger;
        public ChatHub(IMongoDatabase mongoDatabase, ILogger<ChatHub> logger)
        {
            messages = mongoDatabase.GetCollection<Message>("Messages");
            chatrooms = mongoDatabase.GetCollection<ChatRoom>("Chatrooms");
            this.logger = logger;
        }
        public async Task SendMessage(string chatRoomId,string message,string senderId)
        {
            var newMessage = new Message { chatRoomId = chatRoomId, Text = message };
            await messages.InsertOneAsync(newMessage); 
        }
        public override async Task OnConnectedAsync()
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(Context.GetHttpContext().Request.Query["access_token"]);
            var userId = token.Claims.GetValueOrDefault("id");          
            await base.OnConnectedAsync();
        }
        public async Task CreateDialog(string senderId, string chatRoomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId);
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} покинул в чат");
            await base.OnDisconnectedAsync(exception);
        }
      
        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("UserJoined", Context.ConnectionId);
        }

    }
}
