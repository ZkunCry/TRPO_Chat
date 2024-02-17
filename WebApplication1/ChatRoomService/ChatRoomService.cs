﻿using MongoDB.Bson;
using MongoDB.Driver;

namespace WebApplication1.ChatRoomService
{
    public interface IChatRoomService
    {
        Task<List<ChatRoom>> GetChatRoomsForUser(ObjectId userId);
        Task CreateChatRoom(ChatRoom room);
        Task AddMessageToRoom(ObjectId roomId, Message message);
        Task SendMessage(ObjectId roomId, string senderId, string messageText);

    }
    public class ChatRoomService : IChatRoomService
    {
        private readonly IMongoCollection<ChatRoom> _roomsCollection;

        public ChatRoomService(IMongoDatabase database)
        {
            _roomsCollection = database.GetCollection<ChatRoom>("ChatRooms");
        }

        public async Task<List<ChatRoom>> GetChatRoomsForUser(ObjectId userId)
        {
            return await _roomsCollection.Find(r => r.Participants.Contains(new User { _Id= userId })).ToListAsync();
        }

        public async Task CreateChatRoom(ChatRoom room)
        {
            await _roomsCollection.InsertOneAsync(room);
        }

        public async Task AddMessageToRoom(ObjectId roomId, Message message)
        {
            var filter = Builders<ChatRoom>.Filter.Eq(r => r._Id, roomId);
            var update = Builders<ChatRoom>.Update.Push(r => r.Messages, message);
            await _roomsCollection.UpdateOneAsync(filter, update);
        }
        public async Task SendMessage(ObjectId roomId, string senderId, string messageText)
        {
            var message = new Message
            {
                Text = messageText,
                SenderId = senderId,
                SentAt = DateTime.UtcNow
            };

            await AddMessageToRoom(roomId, message);
        }

        public async Task<List<Message>> GetMessagesForRoom(ObjectId roomId)
        {
            var room = await _roomsCollection.Find(r => r._Id == roomId).FirstOrDefaultAsync();
            return room?.Messages ?? new List<Message>();
        }
    }
}
