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
        public async Task<List<RoomModel>> GetAllRooms() =>
            await _roomService.GetAsync();

        [HttpPost("CreateRoom")]
        public async Task<IActionResult> CreateNewRoom(NewRoomDto room)
        {
            if (room == null)
            {
                return BadRequest($"Fehler beim hinzufügen des Raumes (╯°□°）╯︵ ┻━┻");
            }

            try
            {
                RoomModel newRoom = new();
                newRoom.RoomName = room.RoomName;
                await _roomService.CreateAsync(newRoom);
                return Ok("Room successfully created");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create the Room: {ex.Message} (╯°□°）╯︵ ┻━┻");
            }
        }
        [HttpDelete("RemoveRoom")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RemoveRoom(string RoomId)
        {
            await _roomService.RemoveAsync(RoomId);
            return Ok();
        }
    }
}
