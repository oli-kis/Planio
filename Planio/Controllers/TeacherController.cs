using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planio.Models;
using Planio.Services;
using System.Data;

namespace Planio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController : ControllerBase
    {
        private readonly TeachersService _teachersService;
        private readonly ClassService _classService;
        private readonly LessonService _lessonService;

        private readonly IConfiguration _configuration;

        public TeacherController(IConfiguration configuration, TeachersService teachersService, ClassService classService, LessonService lessonService)
        {
            _configuration = configuration;
            _teachersService = teachersService;
            _classService = classService;
            _lessonService = lessonService;
        }

        [HttpGet("GetTeacherSelf")]
        [Authorize(Roles = "teacher")]
        public async Task<IActionResult> GetTeacherSelf()
        {
            TeacherModel teacher = await _teachersService.GetById();
            return Ok(teacher);
        }


        [HttpGet("GetLessonsOfTeacherSelf")]
        [Authorize(Roles = "teacher")]
        public async Task<IActionResult> GetLessonsOfTeacherSelf()
        {
            TeacherModel teacher = await _teachersService.GetById();
            List<LessonModel> lessons = new();
            foreach (var lessonId in teacher.LessonIDs)
            {
                LessonModel lessonToAdd = await _lessonService.GetSingle(lessonId);
                if (lessonToAdd != null)
                {
                    lessons.Add(lessonToAdd);
                }
            }
            return Ok(lessons);
        }

        [HttpGet("GetTeacher")]
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> GetTeacher(string teacherId)
        {
            TeacherModel teacher = await _teachersService.GetSingle(teacherId);
            if(teacher == null) { return NotFound("Der Lehrer wurde nicht gefunden"); }
            return Ok(teacher);
        }


        [HttpGet("GetLessonsOfTeacher")]
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> GetLessonsOfTeacher(string teacherId)
        {
            TeacherModel teacher = await _teachersService.GetSingle(teacherId);
            if(teacher == null) { return NotFound("Der Lehrer wurde nicht gefunden"); }
            List<LessonModel> lessons = new();
            foreach (var lessonId in teacher.LessonIDs)
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
