using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace WebApplication1
{

    public class ChatRoom
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Participants")]
        public List<User> Participants { get; set; }

        [BsonElement("SenderId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string sender { get; set; }

        [BsonElement("OwnerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string owner { get; set; }

    }
}
