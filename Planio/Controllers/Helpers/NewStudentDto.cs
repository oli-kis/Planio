using Planio.Models;

namespace Planio.Controllers.Helpers
{
    public class NewStudentDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ClassName { get; set; }
    }
}
