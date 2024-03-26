using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Planio.Models
{
    public class StudentModel : UserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string  ClassID { get; set; }
        public StudentModel()
        {
            this.Role = "student"; 
        }
    }
}
