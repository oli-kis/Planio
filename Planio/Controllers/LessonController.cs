using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planio.Controllers.Helpers;
using Planio.Models;
using Planio.Services;
using System.Data;

namespace Planio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LessonController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly LessonService _lessonService;
        private readonly TeachersService _teacherService;
        private readonly ClassService _classService;
        private readonly RoomsService _roomsService;

        public LessonController(LessonService lessonService, IConfiguration configuration, TeachersService teacherService, ClassService classService, RoomsService roomsService)
        {
            _configuration = configuration;
            _lessonService = lessonService;
            _teacherService = teacherService;
            _classService = classService;
            _roomsService = roomsService;
        }

        [HttpGet("GetAllLessons")]
        public async Task<List<LessonModel>> GetAllLessons() =>
            await _lessonService.GetAsync();

        [HttpPost("CreateLesson")]
        public async Task<IActionResult> CreateNewLesson(LessonModel lesson)
        {
            if (lesson == null)
            {
                return BadRequest($"Fehler beim hinzufügen der Lektion (╯°□°）╯︵ ┻━┻");
            }
            var teacher = await _teacherService.GetWithEmail(lesson.TeacherMail);
            if (teacher == null)
            {
                return BadRequest($"Lehrer wurde nicht gefunden (╯°□°）╯︵ ┻━┻");
            }
            var classToAdd = await _classService.GetWithClassName(lesson.AttendingClassName);
            if (classToAdd == null)
            {
                return BadRequest($"Klasse wurde nicht gefunden (╯°□°）╯︵ ┻━┻");
            }
            var room = await _roomsService.GetWithRoomName(lesson.RoomName);
            if (room == null)
            {
                return BadRequest($"Raum wurde nicht gefunden (╯°□°）╯︵ ┻━┻");
            }
            if (lesson.LessonTime < 1 || lesson.LessonTime > 40)
            {
                return BadRequest($"Zeit muss zwischen 1 und 40 sein (╯°□°）╯︵ ┻━┻");
            }

            try
            {
                LessonModel newLesson = new();
                newLesson.LessonName = lesson.LessonName;
                newLesson.RoomName = lesson.RoomName;
                newLesson.AttendingClassName = lesson.AttendingClassName;
                newLesson.TeacherMail = lesson.TeacherMail;
                newLesson.LessonTime = lesson.LessonTime;
                await _lessonService.CreateAsync(newLesson);
                await _lessonService.AddLessonToTeacher(newLesson, teacher);
                await _lessonService.AddLessonToClass(newLesson, classToAdd);
                await _roomsService.AddLessonToRoom(room, newLesson);
                return Ok("Lesson successfully created");

            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create the Lesson: {ex.Message} (╯°□°）╯︵ ┻━┻");
            }
        }

        [HttpDelete("RemoveLesson")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RemoveLesson(string LessonId)
        {
            await _lessonService.RemoveAsync(LessonId);
            return Ok();
        }
    }
}
