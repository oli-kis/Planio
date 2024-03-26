using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Planio.Models
{
    public class TeacherModel : UserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public HashSet<string> LessonIDs { get; set; } = new HashSet<string>();
        public TeacherModel()
        {
            this.Role = "teacher";
        }
    }
}
