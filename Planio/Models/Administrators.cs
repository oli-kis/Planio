using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Planio.Models
{
    public class Administrators : UserModel
    {
        public Administrators()
        {
            this.Role = "admin";
        }
    }
}
