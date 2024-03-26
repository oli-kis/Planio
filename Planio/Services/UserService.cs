using Planio.Models;

namespace Planio.Services
{
    public class UserService
    {
        private readonly StudentsService _studentsService;
        private readonly TeachersService _teachersService;
        private readonly AdminService _adminService;

        public UserService(StudentsService studentsService, TeachersService teachersService, AdminService adminService)
        {
            _studentsService = studentsService;
            _teachersService = teachersService;
            _adminService = adminService;
        }

        public async Task<UserModel> GetUserWithEmail(string email)
        {
            var student = await _studentsService.GetWithEmail(email);
            if (student != null)
            {
                return new UserModel
                {
                    Id = student.Id,
                    Email = student.Email,
                    Password = student.Password,
                    Role = student.Role,
                };
            }

            var teacher = await _teachersService.GetWithEmail(email);
            if (teacher != null)
            {
                return new UserModel
                {
                    Id = teacher.Id,
                    Email = teacher.Email,
                    Password = teacher.Password,
                    Role = teacher.Role,
                };
            }
            var admin = await _adminService.GetWithEmail(email);
            if (admin != null)
            {
                return new UserModel
                {
                    Id = admin.Id,
                    Email = admin.Email,
                    Password = admin.Password,
                    Role = admin.Role,
                };
            }
            return null;
        }
    }
}
