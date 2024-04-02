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
    public class RoomController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly LessonService _lessonService;
        private readonly TeachersService _teacherService;
        private readonly ClassService _classService;
        private readonly RoomsService _roomService;

        public RoomController(LessonService lessonService, IConfiguration configuration, TeachersService teacherService, ClassService classService, RoomsService roomService)
        {
            _configuration = configuration;
            _lessonService = lessonService;
            _teacherService = teacherService;
            _classService = classService;
            _roomService = roomService;
        }

        [HttpGet("GetAllRooms")]
        [Authorize(Roles = "admin, teacher")]
        public async Task<List<RoomModel>> GetAllRooms() =>
            await _roomService.GetAsync();

        [HttpPost("CreateRoom")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateNewRoom(NewRoomDto room)
        {
            if (room == null)
            {
                return BadRequest($"Fehler beim Hinzufügen des Raumes (╯°□°）╯︵ ┻━┻");
            }
            if(room.RoomName == "") { return BadRequest("Bitte geben Sie einen Raumnamen ein"); }

            try
            {
                RoomModel newRoom = new()
                {
                    RoomName = room.RoomName
                };
                await _roomService.CreateAsync(newRoom);
                return Ok("Raum wurde erfolgreich hinzugefügt");
            }
            catch (Exception ex)
            {
                return BadRequest($"Fehler beim Erstellen des Raumes: {ex.Message} (╯°□°）╯︵ ┻━┻");
            }
        }
        [HttpDelete("RemoveRoom")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RemoveRoom(string RoomId)
        {
            await _roomService.RemoveAsync(RoomId);
            return Ok("Raum erolgreich gelöscht");
        }
    }
}
