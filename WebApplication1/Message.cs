using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace WebApplication1
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        
        [BsonRepresentation(BsonType.ObjectId)]
        public string chatRoomId { get; set; }  

        [BsonElement("Text")]
        public string Text { get; set; }

        [BsonElement("SenderId")]
        public string SenderId { get; set; }

        [BsonElement("SentAt")]
        public DateTime SentAt { get; set; }
    }
}
