using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Planio.Models
{
    public class ClassModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ClassName { get; set; }
        public HashSet<string> StudentIDs { get; set; } = new HashSet<string>();    
        public HashSet<string> LessonIDs { get; set; } = new HashSet<string>();
    }
}
