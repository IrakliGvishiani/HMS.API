using HMS.Application.Contracts.Service;
using HMS.Application.Models.RoomDtos;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static CommonResponse;

namespace HMS.API.Controllers
{
    [Route("api/room")]
    [ApiController]
    public class RoomController : Controller
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpPost]

        public async Task<IActionResult> CreateRoom([FromBody] RoomForCreatingDto model)
        {
            var result = await _roomService.CreateRoomAsync(model);
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, result, true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(resp.HttpStatusCode, resp);
        }
    }
}
