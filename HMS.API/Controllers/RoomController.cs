using HMS.Application.Contracts.Service;
using HMS.Application.Models.RoomDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using System.Security.Claims;
using static CommonResponse;
using static HMS.API.Examples.HotelForCreatingDtoExample;

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

        [Authorize(Roles = "Manager")]
        [HttpPost]
        [SwaggerRequestExample(typeof(RoomForCreatingDto), typeof(RoomForCreatingDtoExample))]
        public async Task<IActionResult> CreateRoom([FromBody] RoomForCreatingDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _roomService.CreateRoomAsync(model, userId);
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, result, true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(resp.HttpStatusCode, resp);
        }

        [HttpPut]

        public async Task<IActionResult> UpdateRoom([FromBody] RoomForUpdatingDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _roomService.UpdateRoomAsync(model, userId);
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, result, true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(resp.HttpStatusCode, resp);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchRooms([FromQuery] SearchRoomDto model)
        {
            var result = await _roomService.SearchRoomsAsync(model);
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, result, true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(resp.HttpStatusCode, resp);
        }


        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteRoom(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _roomService.DeleteRoomAsync(id, userId);
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, result, true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(resp.HttpStatusCode, resp);
        }

        [HttpGet("{hotelId}")]
        public async Task<IActionResult> GetRoomsByHotelId(int hotelId)
        {
            var result = await _roomService.GetRoomsByHotelIdAsync(hotelId);
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, result, true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(resp.HttpStatusCode, resp);
        }
    }
}