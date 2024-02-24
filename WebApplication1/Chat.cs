using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
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

        [BsonElement("LastMessage")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string lastMesageId { get; set;}
 

    }
}
