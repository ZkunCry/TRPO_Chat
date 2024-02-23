using WebApplication1;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
namespace WebApplication1
{
    public class User
    {
        [BsonId]
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string _Id { get; set; }
        [BsonElement("UserName")]
        public string Name { get; set; }
        [BsonElement("Password")]
        public string Password { get; set; }

    }
   
}
