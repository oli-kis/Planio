using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Planio.Models
{
    public class RoomModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string RoomName { get; set; }    
        public HashSet<string> LessonIDs { get; set; } = new HashSet<string>();
    }
}
