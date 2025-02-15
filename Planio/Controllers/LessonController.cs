﻿using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "admin")]
        public async Task<List<LessonModel>> GetAllLessons() =>
            await _lessonService.GetAsync();

        [HttpPost("CreateLesson")]
        [Authorize(Roles = "teacher")]
        public async Task<IActionResult> CreateNewLesson(NewLessonDto lesson)
        {
                if (lesson == null)
                {
                    return BadRequest($"Fehler beim Hinzufügen der Lektion (╯°□°）╯︵ ┻━┻");
                }
                var teacher = await _teacherService.GetById();
                if (teacher == null)
                {
                    return NotFound($"Lehrer wurde nicht gefunden (╯°□°）╯︵ ┻━┻");
                }
                bool teacherAvailability = await CheckIfTeacherAvailable(teacher, lesson.LessonTime);
                if (!teacherAvailability) { return BadRequest("Lehrer bereits besetzt"); }

                var classToAdd = await _classService.GetWithClassName(lesson.AttendingClassName);
                if (classToAdd == null)
                {
                    return NotFound($"Klasse wurde nicht gefunden (╯°□°）╯︵ ┻━┻");
                }
                bool classAvailability = await CheckIfClassAvailable(classToAdd, lesson.LessonTime);
                if (!classAvailability) { return BadRequest("Klasse bereits besetzt"); }

                var room = await _roomsService.GetWithRoomName(lesson.RoomName);
                if (room == null)
                {
                    return NotFound($"Raum wurde nicht gefunden (╯°□°）╯︵ ┻━┻");
                }
                bool roomAvailability = await CheckIfRoomAvailable(room, lesson.LessonTime);
                if (!roomAvailability) { return BadRequest("Raum bereits belegt"); }

                if (lesson.LessonTime < 1 || lesson.LessonTime > 40)
                {
                    return BadRequest($"Die Zeitangabe muss zwischen 1 und 40 sein (╯°□°）╯︵ ┻━┻");
                }
             
            try
            {
                LessonModel newLesson = new()
                {
                    LessonName = lesson.LessonName,
                    RoomName = lesson.RoomName,
                    AttendingClassName = lesson.AttendingClassName,
                    TeacherMail = teacher.Email,
                    LessonTime = lesson.LessonTime
                };
                await _lessonService.CreateAsync(newLesson);
                await _lessonService.AddLessonToTeacher(newLesson, teacher);
                await _lessonService.AddLessonToClass(newLesson, classToAdd);
                await _roomsService.AddLessonToRoom(room, newLesson);
                return Ok("Lesson successfully created");

            }
            catch (Exception ex)
            {
                return BadRequest($"Fehler beim Erstellen der Lektion: {ex.Message} (╯°□°）╯︵ ┻━┻");
            }
        }

        [HttpDelete("RemoveLesson")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RemoveLesson(string LessonId)
        {
            await _lessonService.RemoveAsync(LessonId);
            return Ok("Lektion erfolgreich entfernt");
        }

        private async Task<bool> CheckIfTeacherAvailable(TeacherModel teacher, int lessonTime)
        {
            foreach (var lessonId in teacher.LessonIDs)
            {
                LessonModel lessonToAdd = await _lessonService.GetSingle(lessonId);
                if (lessonToAdd.LessonTime == lessonTime)
                {
                    return false;
                }
            }
            return true;
        }
        private async Task<bool> CheckIfClassAvailable(ClassModel classToAdd, int lessonTime)
        {
            foreach (var lessonId in classToAdd.LessonIDs)
            {
                LessonModel lessonToAdd = await _lessonService.GetSingle(lessonId);
                if (lessonToAdd == null)
                {
                    return true;
                }
                if (lessonToAdd.LessonTime == lessonTime)
                {
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> CheckIfRoomAvailable(RoomModel room, int lessonTime)
        {
            foreach (var lessonId in room.LessonIDs)
            {
                LessonModel lessonToAdd = await _lessonService.GetSingle(lessonId);
                if (lessonToAdd.LessonTime == lessonTime)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
