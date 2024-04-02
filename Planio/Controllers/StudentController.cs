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
            if (student == null)
            {
                return NotFound("Der Schüler wurde nicht gefunden");
            }
            return Ok(student);
        }

        [HttpGet("GetClassOfStudentSelf")]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> GetClassOfStudentSelf()
        {
            StudentModel student = await _studentsService.GetById();
            if (student == null)
            {
                return NotFound("Der Schüler wurde nicht gefunden");
            }
            var studentClass = await _classService.GetSingle(student.ClassID);
            if (studentClass == null)
            {
                return NotFound("Die Klasse wurde nicht gefunden");
            }
            return Ok(studentClass);
        }


        [HttpGet("GetLessonsOfStudentSelf")]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> GetLessonsOfStudentSelf()
        {
            StudentModel student = await _studentsService.GetById();
            if (student == null){return NotFound("Der Schüler wurde nicht gefunden");}
            var studentClass = await _classService.GetSingle(student.ClassID);
            if (studentClass == null) { return NotFound("Die Klasse wurde nicht gefunden"); }
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
            return NotFound("Der Schüler wurde nicht gefunden");
        }

        [HttpGet("GetClassOfStudent")]
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> GetClassOfStudent(string studentId)
        {
            StudentModel student = await _studentsService.GetSingle(studentId);
            if (student == null)
            {
                return NotFound("Der Schüler wurde nicht gefunden");
            }
            var studentClass = await _classService.GetSingle(student.ClassID);
            if (studentClass != null)
            {
                return Ok(studentClass);
            }
            return NotFound("Die Klasse wurde nicht gefunden");
        }


        [HttpGet("GetLessonsOfStudent")]
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> GetLessonsOfStudent(string studentId)
        {
            StudentModel student = await _studentsService.GetSingle(studentId);
            if (student == null){return NotFound("Der Schüler wurde nicht gefunden");}
            var studentClass = await _classService.GetSingle(student.ClassID);
            if (studentClass == null) { return NotFound("Die Klasse wurde nicht gefunden"); }
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
