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

        [HttpGet("GetStudent"), Authorize]
        public async Task<IActionResult> GetMe()
        {
            StudentModel student = await _studentsService.GetById();
            return Ok(student);
        }

        [HttpGet("GetClassOfStudent")]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> GetClassOfStudent()
        {
            StudentModel student = await _studentsService.GetById();
            var studentClass = await _classService.GetSingle(student.ClassID);
            
            return Ok(studentClass);
        }


        [HttpGet("GetLessonsOfStudent")]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> GetLessonsOfStudent()
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

    }
}
