﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace WebApplication1
{

    public class ChatRoom
    {
        [BsonId]
     
        public ObjectId _Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Participants")]
        public List<User> Participants { get; set; }

        [BsonElement("Messages")]
        public List<Message> Messages { get; set; }
    }
}
