using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Planio.Models
{
    public class LessonModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string LessonName { get; set; }
        public string AttendingClassName { get; set; }
        public string RoomName { get; set; }
        public string TeacherMail { get; set; }

        public int LessonTime { get; set; }
    }
}
