using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planio.Models;
using Planio.Services;

namespace Planio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly StudentsService _studentsService;
        private readonly ClassService _classService;
        private readonly LessonService _lessonService;

        private readonly IConfiguration _configuration;

        public StudentController(IConfiguration configuration, StudentsService studentsService, ClassService classService, LessonService lessonService)
        {
            _configuration = configuration;
            _studentsService = studentsService;
            _classService = classService;
            _lessonService = lessonService;
        }

        [HttpGet("GetStudentSelf")]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> GetMe()
        {
            StudentModel student = await _studentsService.GetById();
            return Ok(student);
        }

        [HttpGet("GetClassOfStudentSelf")]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> GetClassOfStudentSelf()
        {
            StudentModel student = await _studentsService.GetById();
            var studentClass = await _classService.GetSingle(student.ClassID);
            
            return Ok(studentClass);
        }


        [HttpGet("GetLessonsOfStudentSelf")]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> GetLessonsOfStudentSelf()
        {
            StudentModel student = await _studentsService.GetById();
            var studentClass = await _classService.GetSingle(student.ClassID);
            List<LessonModel> lessons = new();
            foreach (var lessonId in studentClass.LessonIDs)
            {
                LessonModel lessonToAdd = await _lessonService.GetSingle(lessonId);
                if (lessonToAdd != null)
                {
                    lessons.Add(lessonToAdd);
                }           
            }
            return Ok(lessons);
        }

        [HttpGet("GetStudentById")]
        [Authorize(Roles = "teacher, admin")]
        public async Task<IActionResult> GetStudentById(string id)
        {
            StudentModel student = await _studentsService.GetSingle(id);
            if (student != null)
            {
                return Ok(student);
            }
            return BadRequest("Student Not Found");
        }

        [HttpGet("GetClassOfStudent")]
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> GetClassOfStudent(string studentId)
        {
            StudentModel student = await _studentsService.GetSingle(studentId);
            if (student == null)
            {
                return BadRequest("Student not Found");
            }
            var studentClass = await _classService.GetSingle(student.ClassID);
            if (studentClass != null)
            {
                return Ok(studentClass);
            }
            return BadRequest("Class not Found");
        }


        [HttpGet("GetLessonsOfStudent")]
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> GetLessonsOfStudent(string studentId)
        {
            StudentModel student = await _studentsService.GetSingle(studentId);
            if (student == null){return NotFound("Student was not Found");}
            var studentClass = await _classService.GetSingle(student.ClassID);
            if (studentClass == null) { return NotFound("Class Not Found"); }
            List<LessonModel> lessons = new();
            foreach (var lessonId in studentClass.LessonIDs)
            {
                LessonModel lessonToAdd = await _lessonService.GetSingle(lessonId);
                if (lessonToAdd != null)
                {
                    lessons.Add(lessonToAdd);
                }
            }
            return Ok(lessons);
        }

    }
}
