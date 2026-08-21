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
        private readonly CommonResponse _commonResponse;
        public RoomController(IRoomService roomService,CommonResponse commonResponse)
        {
            _roomService = roomService;
            _commonResponse = commonResponse;
        }

        [Authorize(Roles = "Manager,Admin")]
        
        [HttpPost]
        [SwaggerRequestExample(typeof(RoomForCreatingDto), typeof(RoomForCreatingDtoExample))]
        public async Task<IActionResult> CreateRoom([FromBody] RoomForCreatingDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _roomService.CreateRoomAsync(model, userId);
            return this.ToActionResult(_commonResponse.Created(result));
        }

        [HttpPut]
        [Authorize(Roles = "Manager,Admin")]
        
        public async Task<IActionResult> UpdateRoom([FromBody] RoomForUpdatingDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _roomService.UpdateRoomAsync(model, userId);
            return this.ToActionResult(_commonResponse.Success(result));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchRooms([FromQuery] SearchRoomDto model)
        {
            var result = await _roomService.SearchRoomsAsync(model);
            return this.ToActionResult(_commonResponse.Success(result));
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager,Admin")]
    
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _roomService.DeleteRoomAsync(id, userId);
            return this.ToActionResult(_commonResponse.NoContent());
        }

        [HttpGet("{hotelId}")]
        public async Task<IActionResult> GetRoomsByHotelId(int hotelId)
        {
            var result = await _roomService.GetRoomsByHotelIdAsync(hotelId);
            return this.ToActionResult(_commonResponse.Success(result));
        }
    }
}