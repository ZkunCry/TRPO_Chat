﻿using Microsoft.AspNetCore.SignalR;
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
        private readonly ConcurrentDictionary<string,string> ConnectedUsers = new ConcurrentDictionary<string,string>();    
        public ChatHub(IMongoDatabase mongoDatabase)
        {
            messages = mongoDatabase.GetCollection<Message>("Messages");
            chatrooms = mongoDatabase.GetCollection<ChatRoom>("Chatrooms");
        }
        public async Task SendMessage(string chatRoomId,string message)
        {
            var newMessage = new Message { chatRoomId = chatRoomId, Text = message };

        }
        public override async Task OnConnectedAsync()
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(Context.GetHttpContext().Request.Headers["Auhtorization"]);
            var userId = token.Claims.GetValueOrDefault("id");
            ConnectedUsers.TryAdd(Context.ConnectionId, userId);
            await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} вошел в чат");
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
        public void SendMessageToRoom(string roomId, string senderId, string messageText)
        {
            Clients.All.SendAsync("ReceiveMessage", roomId, senderId, messageText);
        }
        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("UserJoined", Context.ConnectionId);
        }

    }
}
