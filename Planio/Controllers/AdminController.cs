using Microsoft.AspNetCore.Mvc;
using Planio.Controllers.Helpers;
using Planio.Models;
using Planio.Services;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Planio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly PasswordService _passwordService;
        private readonly IConfiguration _configuration;
        private readonly StudentsService _studentsService;
        private readonly TeachersService _teachersService;
        private readonly ClassService _classService;
        private readonly AdminService _adminService;

        public AdminController(ClassService classService,StudentsService studentsService, TeachersService teachersService, PasswordService passwordService, IConfiguration configuration, AdminService adminService)
        {
            _passwordService = passwordService;
            _configuration = configuration;
            _studentsService = studentsService;
            _teachersService = teachersService;
            _classService = classService;
            _adminService = adminService;
        }

        [HttpGet("GetAllStudents")]
        [Authorize(Roles = "admin")]
        public async Task<List<StudentModel>> GetAllStudents() =>
            await _studentsService.GetAsync();

        [HttpGet("GetAllTeachers")]
        [Authorize(Roles = "admin")]
        public async Task<List<TeacherModel>> GetAllTeachers() =>
            await _teachersService.GetAsync();

        [HttpDelete("RemoveStudent")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RemoveStudent(string StudentId)
        {
            await _studentsService.RemoveAsync(StudentId);
            return Ok("Schüler wurde erfolreich entfernt");
        }

        [HttpDelete("RemoveTeacher")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RemoveTeacher(string TeacherId)
        {
            await _teachersService.RemoveAsync(TeacherId);
            return Ok("Lehrer wurde erfolgreich entfernt");
        }

        [HttpPost("RegisterStudent")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RegisterStudent(NewStudentDto newUser)
        {
            try
            {
                if (newUser.Email == null){return BadRequest("Bitte Email eingeben");}
                if (newUser.FirstName == null) { return BadRequest("Bitte Vornamen eingeben"); }
                if (newUser.LastName == null) { return BadRequest("Bitte Nachnamen eingeben"); }
                if (newUser.Password == null) { return BadRequest("Bitte Passwort eingeben"); }
                var classToAdd = await _classService.GetWithClassName(newUser.ClassName);
                if (classToAdd == null)
                {
                    return NotFound("Die Klasse wurde nicht gefunden");
                }
                StudentModel student = new()
                {
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    Email = HttpUtility.HtmlEncode(newUser.Email),
                    Password = _passwordService.HashPassword(newUser.Password),
                    ClassID = classToAdd.Id
                };

                if (await _studentsService.GetWithEmail(student.Email) != null)
                {
                    return BadRequest("Ein Schüler mit dieser Email existiert bereits (╯°□°）╯︵ ┻━┻");
                }
                await _studentsService.CreateAsync(student);
                await _classService.AddStudentToClassAsync(student, classToAdd);
                return Ok(student);
            }
            catch (Exception ex)
            {
                return BadRequest($"Fehler beim Registrieren des Schülers {ex.Message} (╯°□°）╯︵ ┻━┻");
            }
        }

        [HttpPost("RegisterTeacher")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RegisterTeacher(NewTeacherDto newUser)
        {
            try
            {
                if (newUser.Email == null) { return BadRequest("Bitte Email eingeben"); }
                if (newUser.FirstName == null) { return BadRequest("Bitte Vornamen eingeben"); }
                if (newUser.LastName == null) { return BadRequest("Bitte Nachnamen eingeben"); }
                if (newUser.Password == null) { return BadRequest("Bitte Passwort eingeben"); }
                TeacherModel teacher = new()
                {
                    Email = HttpUtility.HtmlEncode(newUser.Email),
                    Password = _passwordService.HashPassword(newUser.Password),
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName
                };

                if (await _teachersService.GetWithEmail(HttpUtility.HtmlEncode(teacher.Email)) != null)
                {
                    return BadRequest("Ein Lehrer mit dieser Email existiert bereits (╯°□°）╯︵ ┻━┻");
                }
                await _teachersService.CreateAsync(teacher);

                return Ok(newUser);
            }
            catch (Exception ex)
            {
                return BadRequest($"Fehler beim Registrieren des Lehrers {ex.Message} (╯°□°）╯︵ ┻━┻");
            }
        }


        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin(NewAdminDto newAdmin)
        {
            try
            {
                if (newAdmin.Email == null) { return BadRequest("Bitte Email eingeben"); }
                if (newAdmin.Password == null) { return BadRequest("Bitte Passwort eingeben"); }
                Administrators admin = new()
                {
                    Email = HttpUtility.HtmlEncode(newAdmin.Email),
                    Password = _passwordService.HashPassword(newAdmin.Password)
                };

                if (await _adminService.GetWithEmail(HttpUtility.HtmlEncode(admin.Email)) != null)
                {
                    return BadRequest("Ein Admin mit dieser Email existiert bereits (╯°□°）╯︵ ┻━┻");
                }
                await _adminService.CreateAsync(admin);

                return Ok(admin);
            }
            catch (Exception ex)
            {
                return BadRequest($"Fehler beim Registrieren des Admins {ex.Message} (╯°□°）╯︵ ┻━┻");
            }
        }
    }
}
