using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Planio.Controllers.Helpers;
using Planio.Models;
using Planio.Services;
using System.Web;

namespace Planio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ClassService _classService;
        private readonly LessonService _lessonService;

        public ClassController(ClassService classService, IConfiguration configuration, LessonService lessonService)
        {
            _configuration = configuration;
            _classService = classService;
            _lessonService = lessonService;
        }

        [HttpGet("GetAllClasses")]
        [Authorize(Roles = "admin, teacher")]
        public async Task<List<ClassModel>> GetAllClasses() =>
            await _classService.GetAsync();

        [HttpPost("CreateClass")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateNewClass(NewClassDto classData)
        {
            if (classData == null)
            {
                return BadRequest($"Fehler beim hinzufügen der Klasse (╯°□°）╯︵ ┻━┻");
            }
            
            if (await _classService.GetWithClassName(classData.ClassName) != null)
            {
                return BadRequest("Class already exists");
            }

            try
            {
                ClassModel newClass = new ClassModel();

                newClass.ClassName = classData.ClassName;

                await _classService.CreateAsync(newClass);
                return Ok("Class successfully created");

            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create the Class: {ex.Message} (╯°□°）╯︵ ┻━┻");
            }
        }

        [HttpGet("GetLessonsOfClass")]
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> GetLessonsOfClass(string className)
        {
            var classToGet = await _classService.GetWithClassName(className);
            List<LessonModel> lessons = new();
            foreach (var lessonId in classToGet.LessonIDs)
            {
                LessonModel lessonToAdd = await _lessonService.GetSingle(lessonId);
                if (lessonToAdd != null)
                {
                    lessons.Add(lessonToAdd);
                }
            }
            return Ok(lessons);
        }

        [HttpDelete("RemoveClass")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RemoveClass(string classId)
        {
            await _classService.RemoveAsync(classId);
            return Ok();
        }

    }
}
